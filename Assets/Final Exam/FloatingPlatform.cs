using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    // how far you want the platforms to float up and down from original position
    public float offset;
    // the orignal scale that the platform was. Defined in the start event
    Vector3 orignalScale;

    // Where  the platform should float down to. Defined in the start event
    Vector3 destMin;
    // Where  the platform should float up to. Defined in the start event
    Vector3 destMax;

    // variable for how fast the platform should float
    public float p_transition;
    // variable for how fast the platform should shrink and grow
    public float s_transition;
    
    // helper variables for floating and resizing
    int dir;
    int size;

    public AudioSource[] sfx;

    AudioSource grow_s;
    AudioSource shrink_s;

    void Start()
    {
        orignalScale = transform.localScale;
        dir = -1;
        destMax = new Vector3(transform.position.x, transform.position.y + offset, 0);
        destMin = new Vector3(transform.position.x, transform.position.y - offset, 0);
        sfx = GetComponents<AudioSource>();
        shrink_s = sfx[0];
        grow_s = sfx[1];
        shrink_s.Stop();
        grow_s.Stop();
    }

    //helps platform to flip between floating up and down
    void ChangeDir()
    {
        if(dir == -1)
        {
            dir = 1; 
        }
        else if (dir == 1) 
        {
            dir = -1;
        }
    }
    // moves the platforms to destMax
    void FloatUp()
    {
        if (transform.position == destMax)
        {
            ChangeDir();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destMax, p_transition);
        }
    }
    // moves the platforms to destMax
    void FloatDown()
    {
        if (transform.position == destMin)
        {
            ChangeDir();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destMin, p_transition);
        }
    }
    
    // shrinks platform horizontally until scale is (0,1,1)
    void Shrink()
    {
        transform.localScale = Vector3.Lerp(transform.localScale,new Vector3(0,1,1),s_transition);
    }
    // grows platform horizontally until scale is at the original size
    void Grow()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, orignalScale, s_transition);
    }

    // changes the platforms to original size when player falls off
    public void ChangeBack()
    {
        transform.localScale = orignalScale;
    }

    void Update()
    {
        if(dir == 1)
        {
            FloatUp();
        }
        else if(dir == -1)
        {
            FloatDown();
        }

        if(size == 1)
        {
            Grow();
        }
        else if(size == -1)
        {
            Shrink();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            size = -1;
            shrink_s.Play();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            size = 1;
            grow_s.Play();
        }
    }

}
