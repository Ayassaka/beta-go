using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceControl : MonoBehaviour
{
  FixedJoint fj;
  Rigidbody rb;
  public PieceColor pieceColor = PieceColor.white;
  public bool isHeld(){
    return fj.connectedBody != null;
  }

  
  public void hold(Rigidbody by) {
    if (!isHeld()) {
      fj.connectedBody = by;
      EventBus.Publish<removingPieceEvent>(
        new removingPieceEvent(transform.position));
      unfreeze();
    }
  }

  public void release() {
    fj.connectedBody = null;
    EventBus.Publish<addingPieceEvent>(
      new addingPieceEvent(transform.position, pieceColor));
    freeze();
  }

  RigidbodyConstraints rbc;
  bool frozen;
  void freeze() {
    if (!frozen) {
      // Debug.Log("freeze at " + transform.position);
      rbc = rb.constraints;
      rb.constraints = RigidbodyConstraints.FreezeAll;
      frozen = true;
    }
  }

  void unfreeze() {
    if (frozen) {
      // Debug.Log("unfreeze at " + transform.position);
      rb.constraints = rbc;
      frozen = false;
      // Debug.Log("unfreeze to " + rb.constraints);
    }
  }

  // Start is called before the first frame update
  void Start()
  {
    fj = GetComponent<FixedJoint>();
    rb = GetComponent<Rigidbody>();
    frozen = false;
    freeze();
    StartCoroutine(_add_piece_after_winnning_tester_loaded());
  }

  IEnumerator _add_piece_after_winnning_tester_loaded() {
    while (!WinningTester.loaded) {
      yield return null;
    }
    EventBus.Publish<addingPieceEvent>(
      new addingPieceEvent(transform.position, pieceColor));
  }
}
