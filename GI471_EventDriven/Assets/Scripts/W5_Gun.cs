using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class W5_Gun : MonoBehaviour
{
    public enum GunState
    {
        Idle,
        Fire,
        Reload,
    }

    public float damagePerShot;
    public float speedAnimMove;
    public float moveRange;

    public int maxAmmo;
    public int currentAmmo;
    public float delayPerShot;
    public float reloadTime;

    public Image reloadGuide;
    public Text textAmmo;

    private float countTimeShot;
    private float countReloadTime;

    private Vector3 defaultLocalPos;
    private Ray ray;
    private float countSin;
    public Animator animator;
    private GunState gunState;
    private GameObject owner;

    public delegate void DelegateHandleGunState(GunState gunState);
    public event DelegateHandleGunState OnGunStateChange;

    private void Start()
    {
        ray = new Ray();
        defaultLocalPos = this.transform.localPosition;

        reloadGuide.gameObject.SetActive(false);
    }

    public void SetOwner(GameObject newOwner)
    {
        owner = newOwner;
    }

    public void Update()
    {
        UpdateGunState();
        UpdateTextAmmo();
    }

    public void FirePress()
    {
        ChangeState(GunState.Fire);
    }

    public void FireRelease()
    {
        ChangeState(GunState.Idle);
    }

    public void Reload()
    {
        if(currentAmmo < maxAmmo)
        {
            ChangeState(GunState.Reload);
        }
    }

    private void UpdateTextAmmo()
    {
        textAmmo.text = currentAmmo + " / " + maxAmmo;
    }

    private void UpdateGunState()
    {

        switch (gunState)
        {
            case GunState.Idle:
                {
                    break;
                }
            case GunState.Fire:
                {
                    UpdateFire();
                    break;
                }
            case GunState.Reload:
                {
                    UpdateReload();
                    break;
                }
        }
    }

    private void UpdateFire()
    {
        if (countTimeShot >= delayPerShot)
        {
            if (currentAmmo > 0)
            {
                Fire(owner);
                currentAmmo--;
            }
            else
            {
                NotFire();
            }

            countTimeShot = 0.0f;
        }

        countTimeShot += Time.deltaTime;
    }

    private void UpdateReload()
    {
        if(countReloadTime >= reloadTime)
        {
            currentAmmo = maxAmmo;
            ChangeState(GunState.Idle);
        }

        countReloadTime += Time.deltaTime;

        reloadGuide.fillAmount = countReloadTime / reloadTime;
    }

    public void Fire(GameObject owner)
    {
        animator.SetTrigger("FireTrigger");
        RaycastHit hit;
        ray.origin = Camera.main.transform.position;
        ray.direction = Camera.main.transform.forward;
        bool isHit = Physics.Raycast(ray, out hit, 1000);

        if(isHit)
        {
            var receiveDamage = hit.collider.gameObject.GetComponent<W5_ReceiveDamage>();

            if(receiveDamage)
            {
                receiveDamage.TakeDamage(owner, damagePerShot);
            }
        }
    }

    public void NotFire()
    {
        //Notification not fire
    }

    public void UpdateAnimationMove(float speedRatio)
    {
        countSin += Time.deltaTime * (speedAnimMove * speedRatio);

        Vector3 curPos = this.transform.localPosition;
        curPos.y = defaultLocalPos.y + ((Mathf.Sin(countSin) * moveRange));
        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, curPos, Time.deltaTime * 10.0f);
    }

    public void UpdateAnimationFire()
    {

    }

    private void ChangeState(GunState toChange)
    {
        if(gunState != toChange)
        {
            if (OnGunStateChange != null)
                OnGunStateChange(toChange);

            gunState = toChange;

            switch(gunState)
            {
                case GunState.Idle:
                {
                    reloadGuide.gameObject.SetActive(false);
                    break;
                }
                case GunState.Fire:
                {
                    countTimeShot = 0.0f;
                    break;            
                }
                case GunState.Reload:
                {
                    countReloadTime = 0.0f;
                    reloadGuide.gameObject.SetActive(true);
                    reloadGuide.fillAmount = 0.0f;
                    break;
                }
            }
        }
    }
}
