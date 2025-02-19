//using UnityEngine;

//public class OverlapVolumeEstimator : MonoBehaviour
//{
//    [SerializeField] private Collider otherCollider1; // First collider to check for overlap
//    [SerializeField] private Collider otherCollider2; // Second collider to check for overlap
//    [SerializeField] private Collider otherCollider3; // Third collider to check for overlap
//    [SerializeField] private int sampleSize = 1000; // Number of points to sample within the collider


//    public string chargeTag = "Charge";
//    public string gsTag = "GaussianSurface";
//    public float transparency = 0.5f;

//    //public float originalTransparency = gameObject.AddComponent

//    private Renderer chargeRenderer;
//    //chargeRenderer = GetComponent<Renderer>();

//    //void OnSpecificCollisionEnter(Collision collision)
//    //{
//    //    if ((gameObject.tag == chargeTag && collision.gameObject.tag == gsTag) || (gameObject.tag == gsTag && collision.gameObject.tag == chargeTag))
//    //    {
//    //        ChangeTransparency(gameObject, transparency);
//    //    }
//    //}



//    void OnSpecificCollisionExit(Collision collision)
//    {
//        if (collision.gameObject.CompareTag(gsTag) && chargeRenderer != null)
//        {



//        }

//    }

//    void ChangeTransparency(GameObject obj, float alpha)
//    {
//        Renderer renderer = obj.GetComponent<Renderer>();
//        if (renderer != null)
//        {
//            Color color = renderer.material.color;
//            color.a = alpha;
//            renderer.material.color = color;
//        }
//    }

//    private void OnCollisionEnter(Collision collision)
//    {
//        GameObject other = collision.gameObject;
//        if (other.CompareTag(gsTag))
//        {

//            ChangeTransparency(other, transparency);
//        }

//    }



//        void Update()
//        {
//            //float totalOverlapVolume = 0f;

//            //if (otherCollider1 != null && IsOverlapping(otherCollider1))
//            //{
//            //    totalOverlapVolume += EstimateOverlapVolume(otherCollider1);
//            //}

//            //if (otherCollider2 != null && IsOverlapping(otherCollider2))
//            //{
//            //    totalOverlapVolume += EstimateOverlapVolume(otherCollider2);
//            //}

//            //if (otherCollider3 != null && IsOverlapping(otherCollider3))
//            //{
//            //    totalOverlapVolume += EstimateOverlapVolume(otherCollider3);
//            //}

//            //Debug.Log("Total Overlap Volume: " + totalOverlapVolume + " cubic units");
//        }

//        bool IsOverlapping(Collider otherCollider)
//        {
//            // Check if the bounds of both colliders intersect
//            return GetComponent<Collider>().bounds.Intersects(otherCollider.bounds);
//        }

//        float EstimateOverlapVolume(Collider otherCollider)
//        {
//            int overlapCount = 0;
//            Bounds bounds = GetComponent<Collider>().bounds;
//            for (int i = 0; i < sampleSize; i++)
//            {
//                Vector3 point = new Vector3(
//                    Random.Range(bounds.min.x, bounds.max.x),
//                    Random.Range(bounds.min.y, bounds.max.y),
//                    Random.Range(bounds.min.z, bounds.max.z)
//                );

//                if (otherCollider.bounds.Contains(point))
//                    overlapCount++;
//            }

//            float overlapRatio = (float)overlapCount / sampleSize;
//            return overlapRatio * bounds.size.x * bounds.size.y * bounds.size.z; // Calculate volume of the original collider and scale by overlap ratio
//        }
//    }




