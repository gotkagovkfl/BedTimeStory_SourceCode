using DG.Tweening.Core.Easing;
using Manager;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;






/*


최초 작성자는 본인이 아님
초기화 관련, sfx 재생 및 vfx 표시 관련 코드만 수정 및 작성함


*/






public class Projectile : MonoBehaviour
{
    protected Transform t;
    protected Rigidbody rb;

    [SerializeField] protected float lifeTime = 2;


    [SerializeField] SoundEventSO[] soundSO;
    [SerializeField] GameObject prefab_shoot;
    public GameObject hitPrefab;
    public List<GameObject> trails;
    [SerializeField] SoundEventSO explosionSfx;

    void Awake()
    {
        t = transform;
        rb = GetComponent<Rigidbody>();
    }



    private void Start()
    {
        if( prefab_shoot !=null)
        {
            var shootVFX = Instantiate(prefab_shoot, transform.position, Quaternion.identity);
            Destroy(shootVFX,3f);       // 나중에 풀링
        }
        
        StartCoroutine(DestroyCoroutine());
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(lifeTime);

        Explode(transform.position);
    }

    protected void Explode(Vector3 hitPoint)
    {
        // Vector3 offset = (Player.Instance.T.position - transform.position).normalized * 2;
        var hitVFX = Instantiate(hitPrefab, hitPoint, Quaternion.identity);
        SoundManager.Instance.Play(explosionSfx, transform.position);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Damage Enemy
        if( other.TryGetComponent(out Enemy e))
        {
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 damageEffectPos = new Vector3(hitPoint.x, e.damageEffectPos , hitPoint.z);
            float damage = PlayerStats.Instance.AttackPower;

            e.GetDamaged(damage);
            EffectPoolManager.Instance.GetNormalDamageText(damageEffectPos, damage);





            EventManager.Instance.PostNotification(MEventType.EnemyHitted, this, new TransformEventArgs(transform, true));
            soundSO[Random.Range(0, soundSO.Length)].Raise();
            Explode(hitPoint);            
        }
        //StartCoroutine(DestroyParticle(0f));
    }


    public IEnumerator DestroyParticle(float waitTime)
    {

        if (transform.childCount > 0 && waitTime != 0)
        {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(0).transform)
            {
                tList.Add(t);
            }

            while (transform.GetChild(0).localScale.x > 0)
            {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(0).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++)
                {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }

        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
