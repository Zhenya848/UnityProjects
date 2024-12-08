using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    public void Shoot(IGun gun)
    {
        Ray ray = _camera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(50.0f);

        for (int i = 0; i < gun.BulletCount; i++)
        {
            if (gun.BulletClip <= 0)
                break;

            Vector3 distance = (targetPoint - gun.BulletSpawner.position) + new Vector3(Spread(gun.Spread), Spread(gun.Spread));
            GameObject currentBullet = Instantiate(gun.Bullet, gun.BulletSpawner.position, gun.Bullet.transform.rotation);

            currentBullet.transform.forward = distance.normalized;
            currentBullet.GetComponent<Rigidbody>().AddForce(distance.normalized * gun.BulletForce, ForceMode.Impulse);

            gun.BulletClip--;
            Destroy(currentBullet, 1.5f);
        }
    }

    private float Spread(float spread)
    {
        return Random.Range(-spread, spread);
    }
}
