using Player;
using UnityEngine;

public class DashWallBreaker : MonoBehaviour
{
    public float checkRadius = 2.0f;
    public LayerMask wallLayer;

    public void Check()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, checkRadius, wallLayer);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("BW_Blocking"))
                hit.gameObject.SetActive(false);

            if (!hit.TryGetComponent(out ThinWall wall))
                continue;

            wall.BreakWall();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, checkRadius);
    }
}