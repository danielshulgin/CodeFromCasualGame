using System.Collections;
using UnityEngine;

/// <summary>
/// For Debug
/// </summary>
public class PhysicGunl : MonoBehaviour
{
    public Transform target;
    
    public GameObject bulletPrefab;
    
    public float force;
    
    public float bulletLifeTime;
    
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var bullet = Instantiate(bulletPrefab, transform);
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bullet.transform.position = new Vector3(pos.x, pos.y);
            bullet.GetComponent<Rigidbody2D>().AddForce(force * 
                (Vector2)(target.transform.position - bullet.transform.position).normalized);
            StartCoroutine(DeleteObject(bullet));
        }

        if (Input.GetButtonDown("Fire2"))
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.position = new Vector3(pos.x, pos.y);
        }

        force += Input.mouseScrollDelta.y * 30;
    }

    private IEnumerator DeleteObject(GameObject bullet)
    {
        yield return new WaitForSeconds(bulletLifeTime);
        Destroy(bullet);
    }
}
