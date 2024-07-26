using UnityEngine;

public class EllipticCurveCreator : MonoBehaviour
{
    public EllipticCurve EllipticCurveInstance { get; private set; }
    

    private void Start() {
        EllipticCurveInstance = gameObject.AddComponent<EllipticCurve>();
    }
}