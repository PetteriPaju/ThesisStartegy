using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHitEmitter : MonoBehaviour {


    public ParticleSystem particles;

	// Use this for initialization
	void Start () {

        particles.Play();

	}

    public void SetSorting(int i)
    {
        particles.GetComponent<Renderer>().sortingOrder = i;
    }
	
	// Update is called once per frame
	void Update () {


        if (!particles.isPlaying) GameObject.Destroy(this);

	}
}
