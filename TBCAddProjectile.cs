using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;
using Modding;
using Modding.Serialization;
using Modding.Modules;
using Modding.Blocks;
using skpCustomModule;
using System.Collections;
using Vector3 = UnityEngine.Vector3;

namespace TBCStusSpace
{
	[XmlRoot("TBCAddProjectile")]
	[Reloadable]
	public class TBCAddProjectile : BlockModule
	{
        [XmlElement("Gravity")]
        [DefaultValue(0f)]
        [Reloadable]
        public float Gravity;
    }
	public class TBCAddProjectileBehaviour : BlockModuleBehaviour<TBCAddProjectile>
    {
        private AdShootingBehavour adshootingbehavour;
        private AdProjectileScript adprojectilescript;
        private GameObject projectilepool;
        private Transform projectilmultipool;
        private TBCProjectileController tbcprojectilecontroller;
        public float TBCGravity;
        public override void OnSimulateStart()
        {
			base.OnSimulateStart();

            TBCGravity = Module.Gravity;
            adshootingbehavour = GetComponent<AdShootingBehavour>();

            if (StatMaster.isHosting || !StatMaster.isMP || StatMaster.isLocalSim)
            {
                //弾にスクリプトを貼り付ける

                //レベルエディタ、マルチ以外
                projectilepool = GameObject.Find("PHYSICS GOAL");

                foreach (Transform child in projectilepool.transform)
                {

                    if (child.name == "AdProjectile(Clone)(Clone)")
                    {

                        adprojectilescript = child.gameObject.GetComponent<AdProjectileScript>();

                        //ブロック名が同じならスクリプトを貼り付ける
                        if (adshootingbehavour.BlockName == adprojectilescript.BlockName)
                        {

                            tbcprojectilecontroller = child.gameObject.GetComponent<TBCProjectileController>();

                            if (tbcprojectilecontroller == null)
                            {

                                tbcprojectilecontroller = child.gameObject.AddComponent<TBCProjectileController>();
                            }
                            tbcprojectilecontroller.addgravity = TBCGravity;
                        }
                    }
                }

                //レベルエディタ、マルチのとき
                projectilmultipool = GameObject.Find("PManager").transform.Find("Projectile Pool");

                foreach (Transform child in projectilmultipool.transform)
                {

                    if (child.name == "AdProjectile(Clone)(Clone)")
                    {

                        adprojectilescript = child.gameObject.GetComponent<AdProjectileScript>();

                        //ブロック名が同じならスクリプトを貼り付ける
                        if (adshootingbehavour.BlockName == adprojectilescript.BlockName)
                        {

                            tbcprojectilecontroller = child.gameObject.GetComponent<TBCProjectileController>();

                            if (tbcprojectilecontroller == null)
                            {

                                tbcprojectilecontroller = child.gameObject.AddComponent<TBCProjectileController>();
                            }
                            tbcprojectilecontroller.addgravity = TBCGravity;
                        }
                    }
                }
            }

		}

	}

    public class TBCProjectileController : ProjectileScript
    {
        private Rigidbody rigidbody;
        private Vector3 gravityforce = new Vector3(0.0f, -1.0f, 0.0f);
        private Vector3 look;
        private Quaternion lookrotation;
        public float addgravity;
        public void Awake() 
        {
            base.Awake();
            rigidbody = GetComponent<Rigidbody>();

        }
        public override void FixedUpdate()
        {
            if(addgravity != 0.0f)
            {
                rigidbody.AddForce(addgravity * gravityforce, ForceMode.Force);
            }
            if (this.rigidbody.velocity.magnitude == 0.0f)
                return;

            look = new Vector3(this.rigidbody.velocity.normalized.x, this.rigidbody.velocity.normalized.y, this.rigidbody.velocity.normalized.z);
            lookrotation = Quaternion.LookRotation(look, Vector3.up);
            this.transform.rotation = lookrotation;
        }
        public void OnEnable()
        {
        }
        public void OnCollisionEnter()
        { }
        public void Update()
        { }
        public void ValidCollisionOrTrigger()
        { }
    }
}
