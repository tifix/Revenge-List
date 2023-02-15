using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    float speed = 1;
    float timeAlive = 5;
    float t = 0;
    [Range(1,5),SerializeField]private float damage = 8;

    public Vector3 dir = new Vector3(-1,0,0);
    public GameObject destroyVFX;

    void Update()
    {
        t += Time.deltaTime;

        if(t>timeAlive)
        {
            Instantiate<GameObject>(destroyVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        else if(transform.position.y <= transform.GetChild(1).GetComponent<Transform>().position.y)
        {
            Instantiate<GameObject>(destroyVFX, transform.position, Quaternion.identity);
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
            if (other.gameObject.TryGetComponent<PlayerCombat>(out PlayerCombat PC)) PC.ApplyDamage(damage);
        } 
    }
}
