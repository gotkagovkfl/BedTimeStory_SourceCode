using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using TMPro;
using System.Threading;
using System;

public enum PlayerLegState
{
    Idle,
    Walk,
    Run
}

public enum PlayerBodyState
{
    Default,
    Aim,
}


public class PlayerController : MonoBehaviour
{
    // 하체 및 상체 상태
    public PlayerLegState legState;
    public PlayerBodyState bodyState;

    // 상태 전환 이벤트
    Dictionary<PlayerLegState, Action> onLegStateChangeAction;     
    Dictionary<PlayerBodyState, Action> onBodyStateChangeAction;

    // 플레이어 기능
    PlayerInputManager input;
    PlayerStats stats;
    PlayerCamera playerCamera;
    PlayerMovement  playerMove;
    PlayerAnimation animationController;
    Weapon weapon;
    
    //
    public float mouseSense_min => 0.01f;
    public float mouseSense_max => 2f;

    [SerializeField] float mouseSense;
    public float currMouseSense => mouseSense;


    // UI
    [SerializeField] GameObject crossHairUI;


    // SFX
    [Header("SFX")]
    [SerializeField] SoundEventSO aimSfx;
    [SerializeField] SoundEventSO dashSoundSO;
    [SerializeField] SoundEventSO[] sfxs_footstep;
    [SerializeField] SoundEventSO skillUseSfx;
    [SerializeField] private SoundEventSO[] attackEventSOs;
    [SerializeField] private SoundEventSO reloadEventSo;

    private int footStepSfxIdx = 0;
    int shotSfxIdx = 0;


    //=======================================================================
    private void Start()
    {
        Init();
    }
    
    // 기능들의 초기화를 직접 제어한다. 
    public void Init()
    {
        input = GetComponent<PlayerInputManager>();
        stats = GetComponent<PlayerStats>();
        stats.Init();
        animationController  = GetComponent<PlayerAnimation>();
        animationController.Init();
        playerCamera = GetComponent<PlayerCamera>();
        playerCamera.Init();
        playerMove = GetComponent<PlayerMovement>();
        playerMove.Init();
        weapon = GetComponentInChildren<Weapon>();
        //
        mouseSense  = LocalDataManager.GetMouseSense();

        //
        onLegStateChangeAction=new()
        {
            { PlayerLegState.Idle,null},
            { PlayerLegState.Walk,null},
            { PlayerLegState.Run, OnEnterRunningState},
        };
        onBodyStateChangeAction = new()
        {
            { PlayerBodyState.Default,OnExitAimState},
            { PlayerBodyState.Aim,OnEnterAimState},
        };

        UpdateState(); // 기본 상태 초기화
    }

    // 기능들을 현재 상태에 따라 업데이트한다. 
    private void Update()
    {
        if(GamePlayManager.isGamePlaying == false)
        {
            return;
        }
        
        // 플레이어 상태 설정
        UpdateState();
        
        // parameters 
        Vector2 moveVector = input.playerMoveVector;
        float movementSpeed = stats.currMovementSpeed;

        // 하위 기능 설정
        RotateAndMove(legState,bodyState,moveVector,movementSpeed);  // 움직임
        ControlWeapon(bodyState);           // 무기
        stats.OnUpdate(legState,bodyState);  // 스탯
        ControlCamera(legState,bodyState);   // 카메라
        UpdateAnimation(legState,bodyState,moveVector,movementSpeed); // 애니메이션
    }

    //===========================================================================================================
    // 입력에 따라 하체 및 상체 상태 설정
    void UpdateState()
    {
        // ========하체 상태 결정============
        PlayerLegState nextLegState = PlayerLegState.Idle;
        if (input.playerMoveVector != Vector2.zero)
        {
            if (input.dash && stats.CanRun())
            {
                nextLegState = PlayerLegState.Run;
            }
            else
            {
                nextLegState = PlayerLegState.Walk;
            }    
        }
        // 상태 전환 및 전환 이벤트 실행
        if ( legState != nextLegState )
        {
            legState = nextLegState;
            onLegStateChangeAction[legState]?.Invoke();
        }

        // =========상체 상태 설정===========
        PlayerBodyState nextBodyState = input.aim? PlayerBodyState.Aim:PlayerBodyState.Default;
        // 상태 전환 및 전환 이벤트 실행
        if( bodyState != nextBodyState)
        {
            bodyState = nextBodyState;
            onBodyStateChangeAction[bodyState]?.Invoke();
        }
    }


    // 회전, 이동 
    private void RotateAndMove(PlayerLegState currLegState, PlayerBodyState currBodyState,
                                                                Vector2 moveVector, float movementSpeed)
    {
        // 회전 적용
        playerMove.Rotate(currLegState, currBodyState, moveVector, playerCamera.t_mainCam.eulerAngles.y);
        // 이동
        playerMove.Move(currLegState, currBodyState, moveVector, movementSpeed);
    }


    // 감도 설정  
    public void SetMouseSense(float value)
    {
        mouseSense = Mathf.Clamp(value, mouseSense_min, mouseSense_max);
        mouseSense = (float)System.Math.Round(mouseSense,2);

        LocalDataManager.SetSense( mouseSense );
    }



    // 사격, 스킬, 재장전
    void ControlWeapon(PlayerBodyState bodyState )
    {  
        bool isAiming = bodyState == PlayerBodyState.Aim;
        bool toFire = input.fire;
        bool toUseSkill = input.useSkill;
        bool toReload = input.reload;

        // 일반 사격
        if( weapon.TryShoot(isAiming,toFire))
        {
            playerCamera.ApplyRecoil();  //
            PlaySFX_fire();
        }
        // 유탄 발사
        if( weapon.TryUseSkill(isAiming,toUseSkill))
        {
            playerCamera.ApplyRecoil(1.5f);     // 사격보다 반동이 쏌
            SoundManager.Instance.Play(skillUseSfx, Player.Instance.T.position);   // 스킬 sfx
        }
        // 재장전
        if( weapon.TryReload(isAiming,toReload,toFire))
        {
            SoundManager.Instance.Play(reloadEventSo, Player.Instance.T.position);  // 장전 sfx
        }
    }

    // 카메라 위치 설정 및 회전
    void ControlCamera(PlayerLegState currLegState, PlayerBodyState currBodyState)
    {
        playerCamera.ControlCameraPosition(currLegState,currBodyState); 
        playerCamera.ControlCameraRotation( input.mouseMoveVector, mouseSense);
    }

    // 애니메이션
    void UpdateAnimation(PlayerLegState currLegState, PlayerBodyState currBodyState, 
                                    Vector2 moveVector, float movementSpeed)
    {
        float animSpeed = stats.currMoveSpeedRatio;
        animationController.OnSetBodyState(currBodyState,animSpeed);

        // 애니메이션 (상태별로)
        if( bodyState== PlayerBodyState.Default)
        {
            animationController.OnMove_Default(movementSpeed);
        }
        else if(bodyState == PlayerBodyState.Aim)
        {
            Vector2 normalizedVector = moveVector.normalized;
            animationController.OnMove_OnAim(normalizedVector);
        }
    }









    // 발자국 소리 - 애니메이션 이벤트에 의해 자동으로 호출 
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            var sfx = sfxs_footstep[footStepSfxIdx++];
            SoundManager.Instance.Play(sfx, Player.Instance.T.position);
            
            if (sfxs_footstep.Length <= footStepSfxIdx)
                footStepSfxIdx = 0;
        }
    }

    // 사격 sfx
    void PlaySFX_fire()
    {
        var soundData = attackEventSOs[shotSfxIdx ++];
        SoundManager.Instance.Play(soundData, Player.Instance.T.position);

        if (shotSfxIdx  >= attackEventSOs.Length)
            shotSfxIdx  = 0;

    }



    //========================================
    public void OnVictory()
    {
        animationController.OnGameFinisehd();
    }


    public void OnDefeated()
    {
        animationController.OnGameFinisehd();
    }




    
    #region ==== Events ====
    //
    void OnEnterRunningState()
    {
        SoundManager.Instance.Play(dashSoundSO,Player.Instance.T.position);
    }


    // 조준 시작
    void OnEnterAimState()
    {
        SoundManager.Instance.Play(aimSfx, Player.Instance.T.position); 

        crossHairUI.SetActive(true);    
        playerCamera.OnAim(true);
    }

    // 조준 해제
    void OnExitAimState()
    {
        crossHairUI.SetActive(false);
        playerCamera.OnAim(false);
    }
    #endregion
}
