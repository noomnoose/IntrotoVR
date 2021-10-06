using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class Walk : MonoBehaviour
{

    public Transform P;

    private Animator _Anim;
    private NavMeshAgent _Nav;
    private int i=2;

    private Vector3 oldPos = new Vector3();
    private float dis = 0;

	// Use this for initialization
	void Start ()
	{
        oldPos = transform.position;
        _Anim = this.GetComponent<Animator>();
        _Nav = this.GetComponent<NavMeshAgent>();
    }
	
	// Update is called once per frame
	void Update ()
	{

	}

    void FixedUpdate()
    {
        dis = Vector3.Distance(oldPos,transform.position);
        if (dis>0)
        {

        }
        _Anim.SetFloat("Dis",_Nav.remainingDistance);
    }

    public void StartWalk()
    {
        InvokeRepeating("WalkFree", 0, i);
    }

    void WalkFree()
    {
        if (_Anim)
        {
            i = Random.Range(0, 5);
            if (i>2)
            {
                float X = Random.Range(-10f,10f);
                float Z = Random.Range(-10f, 10f);
                _Nav.SetDestination(new Vector3(X, 0, Z));
            }
        }
    }
}
