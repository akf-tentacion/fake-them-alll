using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;
using AKUtil;

public class ParticlePool : MonoBehaviour
{

    ComponentPool<ParticlePool> pool;
    public ParticleSystem[] particles;
    Transform target;
    float time;
    float deleteTime;

    bool isFirstStart = true;

    void OnEnable()
    {
        if (!isFirstStart) return;
        isFirstStart = false;
        if (particles[0] == null) particles[0] = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (time > deleteTime)
        {
            pool?.Return(this);
        }
        time += Time.deltaTime;
        if (target != null) this.transform.position = target.transform.position;
    }

    public void Play(float deleteTime, ComponentPool<ParticlePool> pool, Transform target)
    {
        if (particles[0] == null)
        {
            particles[0] = GetComponent<ParticleSystem>();
        }
        time = 0;
        this.deleteTime = deleteTime;
        this.pool = pool;
        this.target = target;

        foreach (var particle in particles)
        {
            particle.Stop();
            particle.Play();
        }
    }
}
