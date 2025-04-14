using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using UnityEngine;
using Modding;
using Modding.Serialization;
using Modding.Modules;
using Modding.Blocks;
using skpCustomModule;
using Vector3 = UnityEngine.Vector3;

namespace TBCStusSpace
{
    public class AdAmmunitionStorage : BlockScript
    {
        public float Range = 10f;
        public LayerMask layermask = (1 << 0) | (1 << 12) | (1 << 14) | (1 << 25) | (1 << 26);
        private TBCAddProjectileBehaviour TBCapb;
        private Transform[] ChildObjects;
        private Transform GOparent;
        public override void OnBlockPlaced()
        {
            base.OnBlockPlaced();
        }
        public override void OnSimulateStart()
        {
            base.OnSimulateStart();
            GOparent = this.transform;
            while(GOparent.gameObject.name != "Simulation Machine")
            {
                GOparent = GOparent.transform.parent;
            }

                ChildObjects = GOparent.GetComponentsInChildren<Transform>();
                foreach(Transform childobject in ChildObjects)
                {
                    TBCapb = childobject.GetComponent<TBCAddProjectileBehaviour>();
                    if(TBCapb != null)
                    {
                        TBCapb.TBCAmmoStock += 10;
                        TBCapb.start = true;
                    }
                }
        }
    }
}
