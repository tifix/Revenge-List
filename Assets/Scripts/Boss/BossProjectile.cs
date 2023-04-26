using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    float speed = 1;
    float timeAlive = 5;
    float t = 0;
    [SerializeField] float zDepthMax = 8;
    [SerializeField] float xTravelMax = -12;
    [Range(1,50),SerializeField]float damage = 8;
    [SerializeField]bool isShakingCameraOnHit = false;
    [SerializeField] bool imOverHead = false;

    public Vector3 dir = new Vector3(-1,0,0);
    public GameObject destroyVFX;
    public AudioClip hitSFX;

    private void Awake()
    {
        if (hitSFX == null) Debug.LogWarning("Missing SFX asset for projectile: "+name);//Assigning if reference breaks ...somehow
    }

    void Update()
    {
        t += Time.deltaTime;

        if(t>timeAlive || transform.position.z >= zDepthMax || transform.localPosition.x <= xTravelMax)
        {
            Instantiate<GameObject>(destroyVFX, transform.position, Quaternion.identity);   //despawning after time expires
            Destroy(gameObject);
        }

        else if(imOverHead && transform.position.y <= transform.GetChild(1).GetComponent<Transform>().position.y) //despawning after hitting the floor
        {

            AudioManager.instance.sfxSource.PlayOneShot(hitSFX);
            Instantiate<GameObject>(destroyVFX, transform.position, Quaternion.identity);
            if(isShakingCameraOnHit)GameManager.instance.CallShake(5, 0.5f);
            Destroy(gameObject);
        }

        transform.Translate(dir * speed * Time.deltaTime);
    }

    public void SetSpeed(float s) { speed = s; }
    public void SetDistance(float d) { timeAlive= d; }
    public void SetDirection(Vector3 d) { dir = d; }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player")) 
        {
            Debug.Log("Player hit!");
            if (other.gameObject.TryGetComponent<PlayerCombat>(out PlayerCombat Pc)) Pc.ApplyDamage(damage);
        } 
    }
}
