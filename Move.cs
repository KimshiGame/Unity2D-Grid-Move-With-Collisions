using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : RaycastController
{
    public CollisionInfo collisions;

    bool collideFromLeft;
    bool collideFromTop;
    bool collideFromRight;
    bool collideFromBottom;

    public Transform topWallCheck;
    public Transform bottomWallCheck;
    public Transform rightWallCheck;
    public Transform leftWallCheck;
    public float wallCheckRadius;
    public LayerMask Wall;

    RaycastHit2D hit;
    Vector2 input;

    bool canmove = true; //indicate if a keyboard key can move a piece
    Vector3 targetPosition; //temporary value for moving (used in coroutines)
    public int speed = 3;
    public float gridSize = 1.0f;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (!collideFromTop)
        {
            if (input.y > 0 && canmove == true)
            {
                canmove = false;
                StartCoroutine(MoveInGrid((float)System.Math.Round(transform.position.x, 2), (float)System.Math.Round(transform.position.y + gridSize, 2), (float)System.Math.Round(transform.position.z, 2)));
            }
        }
        else
        {
            input.y = +(Input.GetAxisRaw("Vertical"));
        }
        if (!collideFromRight)
        {
            if (input.x > 0 && canmove == true)
            {
                canmove = false;
                StartCoroutine(MoveInGrid((float)System.Math.Round(transform.position.x + gridSize, 2), (float)System.Math.Round(transform.position.y, 2), (float)System.Math.Round(transform.position.z, 2)));
            }
        }
        else
        {
            input.x = +(Input.GetAxisRaw("Horizontal"));
            //input.x = +(input.x);
        }
        if (!collideFromLeft)
        {
            if (input.x < 0 && canmove == true)
            {
                canmove = false;
                StartCoroutine(MoveInGrid((float)System.Math.Round(transform.position.x - gridSize, 2), (float)System.Math.Round(transform.position.y, 2), (float)System.Math.Round(transform.position.z, 2)));
            }
        }
        else
        {
            input.x = +(Input.GetAxisRaw("Horizontal"));
        }
        if (!collideFromBottom)
        {
            if (input.y < 0 && canmove == true)
            {
                canmove = false;
                StartCoroutine(MoveInGrid((float)System.Math.Round(transform.position.x, 2), (float)System.Math.Round(transform.position.y - gridSize, 2), (float)System.Math.Round(transform.position.z, 2)));
            }
        }
        else
        {
            input.y = +(Input.GetAxisRaw("Vertical"));
        }
        //targetPosition.y = transform.position.y;
    }

    void FixedUpdate()
    {
        collideFromTop = Physics2D.OverlapCircle(topWallCheck.position, wallCheckRadius, Wall);

        collideFromBottom = Physics2D.OverlapCircle(bottomWallCheck.position, wallCheckRadius, Wall);

        collideFromLeft = Physics2D.OverlapCircle(leftWallCheck.position, wallCheckRadius, Wall);

        collideFromRight = Physics2D.OverlapCircle(rightWallCheck.position, wallCheckRadius, Wall);
    }

    IEnumerator MoveInGrid(float x, float y, float z)
    {
        UpdateRaycastOrigins();

        collisions.Reset();
        collisions.moveAmountOld = targetPosition;

        if (targetPosition.x != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(targetPosition.x);
        }

        HorizontalCollisions();

        if (targetPosition.y != 0)
        {
            collisions.faceDir = (int)Mathf.Sign(targetPosition.y);
        }

        VerticalCollisions();

        while (transform.position.x != x || transform.position.y != y || transform.position.z != z)
        {
            //moving x forward
            if (transform.position.x < x)
            {
                //moving the point by speed 
                targetPosition.x = speed * Time.deltaTime;
                //check if the point goes more than it should go and if yes clamp it back
                if (targetPosition.x + transform.position.x > x)
                {
                    targetPosition.x = x - transform.position.x;
                }
            }
            //moving x backward
            else if (transform.position.x > x)
            {
                //moving the point by speed 
                targetPosition.x = -speed * Time.deltaTime;
                //check if the point goes more than it should go and if yes clamp it back
                if (targetPosition.x + transform.position.x < x)
                {
                    targetPosition.x = -(transform.position.x - x);
                }
            }
            else //x is unchanged so should be 0 in translate function
            {
                targetPosition.x = 0;
            }
            //moving y forward
            if (transform.position.y < y)
            {
                //moving the point by speed 
                targetPosition.y = speed * Time.deltaTime;
                //check if the point goes more than it should go and if yes clamp it back
                if (targetPosition.y + transform.position.y > y)
                {
                    targetPosition.y = y - transform.position.y;
                }
            }
            //moving y backward
            else if (transform.position.y > y)
            {
                //moving the point by speed 
                targetPosition.y = -speed * Time.deltaTime;
                //check if the point goes more than it should go and if yes clamp it back
                if (targetPosition.y + transform.position.y < y)
                {
                    targetPosition.y = -(transform.position.y - y);
                }
            }
            else //y is unchanged so it should be zero
            {
                targetPosition.y = 0;
            }
            //moving z forward
            if (transform.position.z < z)
            {
                //moving the point by speed 
                targetPosition.z = speed * Time.deltaTime;
                //check if the point goes more than it should go and if yes clamp it back
                if (targetPosition.z + transform.position.z > z)
                {
                    targetPosition.z = z - transform.position.z;
                }
            }
            //moving z backward
            else if (transform.position.z > z)
            {
                //moving the point by speed 
                targetPosition.z = -speed * Time.deltaTime;
                //check if the point goes more than it should go and if yes clamp it back
                if (targetPosition.z + transform.position.z < z)
                {
                    targetPosition.z = -(transform.position.z - z);
                }
            }
            else //z is unchanged so should be zero in translate function
            {
                targetPosition.z = 0;
            }

            transform.Translate(targetPosition);

            yield return 0;
        }
        //the work is ended now congratulation
        canmove = true;
    }

    void HorizontalCollisions()
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(targetPosition.x) + skinWidth;

        if (Mathf.Abs(targetPosition.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);
        }
    }

    void VerticalCollisions()
    {
        float directionY = Mathf.Sign(targetPosition.y);
        float rayLength = Mathf.Abs(targetPosition.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {

            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + targetPosition.x);
            hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public Vector2 moveAmountOld;
        public int faceDir;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
