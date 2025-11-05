using System.Collections;
using System.Collections.Generic;   
using UnityEngine;

public class Boss_Weapon : MonoBehaviour
{
    public int damage = 20;
    public int magicDamage = 30;

    public Vector3 attackOffset;
    public float attackrange = 0.5f;
    public LayerMask attackMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //public void Attack()
    //{
    //    Vector3 pos = transform.position;
    //    pos += transform.right * attackOffset.x;
    //    pos += transform.up * attackOffset.y;

    //    Collider2D colInfo = Physics2D.OverlapCircle(pos, attackrange, attackMask);
    //    if (colInfo != null)
    //    {
    //        if (colInfo.CompareTag("DrawCharacter"))
    //        {
    //            colInfo.GetComponent<>(DrawCharacter).TakeDamage(damage);
    //        }
            
    //    }

    }

    // Update is called once per frame
    //public void SecondPhaseAttack()
    //{ Vector3 pos = transform.position;
    //    pos += transform.right * attackOffset.x;
    //    pos += transform.up * attackOffset.y;
    //    Collider2D colInfo = Physics2D.OverlapCircle(pos, attackrange, attackMask);
    //    if (colInfo != null)
    //    {
    //        if (colInfo.CompareTag("DrawCharacter"))
    //        {
    //            colInfo.GetComponent<>().TakeMagicDamage(magicDamage);
    //        }

    //    }

    //}
//    private void OnDrawGizmosSelected()
//    {
//        Vector3 pos = transform.position;
//        pos += transform.right * attackOffset.x;
//        pos += transform.up * attackOffset.y;
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(pos, attackrange);
//    }
//}
