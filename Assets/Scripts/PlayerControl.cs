using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
  Gamepad gp;
  Rigidbody rb;
  public float speed = 2;
  public float force = 10;
  public float carryForceMultiplier = 1.5f;
  public float maxPickUpDistance = .2f;

  public KeyCode leftKey = KeyCode.LeftArrow;
  public KeyCode rightKey = KeyCode.RightArrow;
  public KeyCode upKey = KeyCode.UpArrow;
  public KeyCode downKey = KeyCode.DownArrow;
  public KeyCode holdKey = KeyCode.Space;

  public PieceColor pieceColor;

  // Start is called before the first frame update
  void Start()
  {
    // gp = Gamepad.current;
    rb = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update() {
    if (gp != null) {
      if (gp.aButton.wasPressedThisFrame) pick();
      if (gp.aButton.wasReleasedThisFrame) drop();
    } else {
      if (Input.GetKeyDown(holdKey)) pick();
      if (Input.GetKeyUp(holdKey)) drop();
    }
  }

  private void FixedUpdate() {
    if (gp != null) {
      handleMovements(gp.leftStick.ReadValue());
    } else {
      handleMovements(getKeyboardAxis());
    }
  }
  Vector2 getKeyboardAxis() {
    Vector2 v = new Vector2(
        ((Input.GetKey(leftKey)) ? -1 : 0) + ((Input.GetKey(rightKey)) ? 1 : 0),
        ((Input.GetKey(downKey)) ? -1 : 0) + ((Input.GetKey(upKey)) ? 1 : 0)
    );
    if (v != Vector2.zero) v.Normalize();
    return v;
  }

  public float rotateTorque = 3f;
  float _prev_deltaZ = 0f;
  void handleMovements(Vector2 input) {
    rb.AddForce(force * input * Time.fixedDeltaTime);
    if (rb.velocity.magnitude > speed) {
      rb.velocity = rb.velocity.normalized * speed;
    }

    if (input.magnitude > .2f) {
      float targetDirection = directionOf(input);
    
      float _deltaZ = Mathf.Repeat((targetDirection - transform.rotation.eulerAngles.z) + 180, 360) - 180;
      // Debug.Log(_deltaZ);
      if (_prev_deltaZ * _deltaZ < 0) {
        rb.angularVelocity = Vector3.zero;
        // Debug.Log("stopped");
      } else {
        rb.AddTorque(_deltaZ * Mathf.Abs(_deltaZ) / 180 * Time.fixedDeltaTime * rotateTorque * Vector3.forward);
      }
      _prev_deltaZ = _deltaZ;
    } else {
      _prev_deltaZ = 0;
    }


    // if (rb.velocity.magnitude > 1f) transform.up = rb.velocity;
  }


  static float directionOf(Vector2 v) {
    return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
  }

  public LayerMask PieceLayer;
  PieceControl picked;
  void pick() {
    RaycastHit hit;
    if (Physics.Raycast(transform.position, transform.right, out hit, maxPickUpDistance)) {
      PieceControl picking = hit.collider.gameObject.GetComponent<PieceControl>();
      if (!picking.isHeld() && picking.pieceColor == pieceColor) {
        picking.hold(rb);
        picked = picking;
        force = force * carryForceMultiplier;
      }
    }
  }

  void drop() {
    if (picked != null) {
      picked.release();
      picked = null;
      force = force / carryForceMultiplier;
    }
  }


}
