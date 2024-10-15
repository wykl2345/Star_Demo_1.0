using System;
using System.Collections;
using System.Collections.Generic;
using MapStateManager;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5;
    public Animator anim;
    private Rigidbody2D _rb2;
    public Transform movePoint;
    public SpriteRenderer playerRenderer;
    public LayerMask stopMask;
    private Transform _mTrans;
    // Start is called before the first frame update
    void Start()
    {
        _rb2 = GetComponent<Rigidbody2D>();
        _mTrans = transform;
        movePoint.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 move = _mTrans.position;
        
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        anim.SetFloat("XRun",Math.Abs(x));
        
        if (x != 0 || y != 0)
        {
            var offset = new Vector3(x, y);
            
            movePoint.position = PosFix(offset) + move;

            var target = MapManager.Instance.GetTile(movePoint.position);

            move += new Vector3(x, y);

            if (x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }else if (x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

            _rb2.velocity = offset * (moveSpeed * Time.deltaTime);
        }
        else
        {
            _rb2.velocity = new Vector2(0,0);
        }
        
        
        //_mTrans.position = Vector3.MoveTowards(_mTrans.position, move , moveSpeed * Time.deltaTime);
    }

    Vector3 PosFix(Vector3 pos)
    {
        var fixedPos = Vector3.zero; 
        if (pos.x > 0)
        {
            fixedPos.x = 1;
        }else if (pos.x < 0)
        {
            fixedPos.x = -1;
        }

        if (pos.y > 0)
        {
            fixedPos.y = 1;
        }else if (pos.y < 0)
        {
            fixedPos.y = -1;
        }

        return fixedPos;
    }
}
