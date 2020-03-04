using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningTester : MonoBehaviour
{
  GameRules gr;
  public static bool loaded = false;
  PieceColor[,] board;

  // Start is called before the first frame update
  void Start() {
    gr = GameRules.instance;

    board = new PieceColor[gr.boardSize, gr.boardSize];
    for (int x = 0; x < gr.boardSize; x++) {
      for (int y = 0; y < gr.boardSize; y++) {
        board[x, y] = PieceColor.none;
      }
    }

    EventBus.Subscribe<addingPieceEvent>(_onAddingPiece);
    EventBus.Subscribe<removingPieceEvent>(_onRemovingPiece);
    loaded = true;
  }

  int toBoardAxis(float x) {
    return Mathf.RoundToInt(x / gr.boardUnit + (gr.boardSize - 1) / 2);
  }

  float toPiecePosition(int x) {
    return (x - (gr.boardSize - 1) / 2) * gr.boardUnit;
  }

  void setPiece(Vector3 position, PieceColor pieceColor) {
    int x = toBoardAxis(position.x);
    int y = toBoardAxis(position.y);
    if (Vector3.Distance(
        new Vector3(toPiecePosition(x), toPiecePosition(y), 0),
        position) < gr.pieceRadius) {
      board[x, y] = pieceColor;
      // Debug.Log("set piece " + pieceColor + " at (" + x + ", " + y +")");
      winTest(pieceColor);
    } else {
      // Debug.Log("set at no where");
    }
  }

  void winTest(PieceColor c) {
    if (c == PieceColor.none) return;
    for (int x = 0; x < gr.boardSize; x++) {
      _consecutive = 0;
      for (int y = 0; y < gr.boardSize; y++) {
        test_consecutive(x, y, c);
      }
    }

    for (int y = 0; y < gr.boardSize; y++) {
      _consecutive = 0;
      for (int x = 0; x < gr.boardSize; x++) {
        test_consecutive(x, y, c);
      }
    }
    
    for (int x = 0; x < gr.boardSize; x++) {
      _consecutive = 0;
      for (int y = 0; y < gr.boardSize - x; y++) {
        test_consecutive(x + y, y, c);
      }
      _consecutive = 0;
      for (int y = 0; y <= x; y++) {
        test_consecutive(x - y, y, c);
      }
    }

    for (int x = 0; x < gr.boardSize; x++) {
      _consecutive = 0;
      for (int y = 0; y < gr.boardSize - x; y++) {
        test_consecutive(x + y, gr.boardSize - 1 - y, c);
      }
      _consecutive = 0;
      for (int y = 0; y <= x; y++) {
        test_consecutive(x - y, gr.boardSize - 1 - y, c);
      }
    }
    
  }

  int _consecutive;
  void test_consecutive(int x, int y, PieceColor c) {
    if (board[x, y] == c) {
      _consecutive++;
      if (_consecutive == gr.goal) {
        EventBus.Publish<winningEvent>(new winningEvent(c));
      }
    } else {
      _consecutive = 0;
    }
  }

  void _onAddingPiece(addingPieceEvent e) {
    setPiece(e.getPosition(), e.getColor());
  }
  void _onRemovingPiece(removingPieceEvent e) {
    setPiece(e.getPosition(), PieceColor.none);
  }

}

public enum PieceColor
{
  none,
  white,
  black
}
public class addingPieceEvent {
  PieceColor color;
  Vector3 position;
  public PieceColor getColor() {
    return color;
  }
  public Vector3 getPosition() {
    return position;
  }
  public addingPieceEvent(Vector3 position, PieceColor pieceColor) {
    this.color = pieceColor;
    this.position = position;
  }
}
public class removingPieceEvent {
  // PieceColor color;
  Vector3 position;
  // public PieceColor getColor() {
  //     return color;
  // }
  public Vector3 getPosition() {
    return position;
  }
  public removingPieceEvent(/* PieceColor pieceColor, */Vector3 position) {
    this.position = position;
  }
}

public class winningEvent {
  PieceColor color;

  public PieceColor getColor() {
    return color;
  }
  public winningEvent(PieceColor pieceColor) {
    this.color = pieceColor;
  }

}
