using UnityEngine;
using System.Collections;

public class Piece
{
    public enum PieceType
    {
        I,
        J,
        L,
        O,
        S,
        T,
        Z
    }
	
    static byte[,,] pieceDefI = new byte[,,]
                              {
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 1, 2, 3, 4}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }, 
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 1, 0, 0}, 
                                      {0, 0, 2, 0, 0}, 
                                      {0, 0, 3, 0, 0}, 
                                      {0, 0, 4, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 0, 0, 0},
                                      {4, 3, 2, 1, 0},
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 4, 0, 0}, 
                                      {0, 0, 3, 0, 0}, 
                                      {0, 0, 2, 0, 0}, 
                                      {0, 0, 1, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }
                              };

    static byte[,,] pieceDefO = new byte[,,]
                              {
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 1, 2, 0}, 
                                      {0, 0, 3, 4, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }, 
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 3, 1, 0}, 
                                      {0, 0, 4, 2, 0}, 
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 4, 3, 0},
                                      {0, 0, 2, 1, 0},
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 2, 4, 0}, 
                                      {0, 0, 1, 3, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }
                              };

    static byte[, ,] pieceDefL = new byte[,,]
                              {
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 1, 0, 0}, 
                                      {0, 0, 2, 0, 0}, 
                                      {0, 0, 3, 4, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }, 
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 3, 2, 1, 0}, 
                                      {0, 4, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0},
                                      {0, 4, 3, 0, 0},
                                      {0, 0, 2, 0, 0},
                                      {0, 0, 1, 0, 0},
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 4, 0}, 
                                      {0, 1, 2, 3, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }
                              };


    static byte[, ,] pieceDefJ = new byte[,,]
                              {
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 1, 0, 0}, 
                                      {0, 0, 2, 0, 0}, 
                                      {0, 4, 3, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }, 
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 4, 0, 0, 0}, 
                                      {0, 3, 2, 1, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 3, 4, 0},
                                      {0, 0, 2, 0, 0},
                                      {0, 0, 1, 0, 0},
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 1, 2, 3, 0}, 
                                      {0, 0, 0, 4, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }
                              };

    static byte[, ,] pieceDefS = new byte[,,]
                              {
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 3, 4, 0}, 
                                      {0, 1, 2, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }, 
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 1, 0, 0, 0}, 
                                      {0, 2, 3, 0, 0}, 
                                      {0, 0, 4, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 2, 1, 0},
                                      {0, 4, 3, 0, 0},
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 4, 0, 0}, 
                                      {0, 0, 3, 2, 0}, 
                                      {0, 0, 0, 1, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }
                              };

    static byte[, ,] pieceDefZ = new byte[,,]
                              {
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 1, 2, 0, 0}, 
                                      {0, 0, 3, 4, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }, 
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 1, 0, 0}, 
                                      {0, 3, 2, 0, 0}, 
                                      {0, 4, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0},
                                      {0, 4, 3, 0, 0},
                                      {0, 0, 2, 1, 0},
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 4, 0}, 
                                      {0, 0, 2, 3, 0}, 
                                      {0, 0, 1, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }
                              };

    static byte[, ,] pieceDefT = new byte[,,]
                              {
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 4, 0, 0}, 
                                      {0, 1, 2, 3, 0}, 
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }, 
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 1, 0, 0}, 
                                      {0, 0, 2, 4, 0}, 
                                      {0, 0, 3, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0},
                                      {0, 0, 0, 0, 0},
                                      {0, 3, 2, 1, 0},
                                      {0, 0, 4, 0, 0},
                                      {0, 0, 0, 0, 0}
                                  },
                                  {
                                      {0, 0, 0, 0, 0}, 
                                      {0, 0, 1, 0, 0}, 
                                      {0, 4, 2, 0, 0}, 
                                      {0, 0, 3, 0, 0}, 
                                      {0, 0, 0, 0, 0}
                                  }
                              };



    public PieceType currentPieceType;
    private int currentRotationIndex;

    public Piece()
    {
        currentRotationIndex = 0;

        int randVal = Random.Range(0, 7);

        if (randVal == 0)
        {
            currentPieceType = PieceType.I;
        }
        else if (randVal == 1)
        {
            currentPieceType = PieceType.O;
        }
        else if (randVal == 2)
        {
            currentPieceType = PieceType.J;
        }
        else if (randVal == 3)
        {
            currentPieceType = PieceType.L;
        }
        else if (randVal == 4)
        {
            currentPieceType = PieceType.S;
        }
        else if (randVal == 5)
        {
            currentPieceType = PieceType.Z;
        }
        else if (randVal >= 6)
        {
            currentPieceType = PieceType.T;
        }

    }

    public void IncrementRotation()
    {
        currentRotationIndex++;
        if (currentRotationIndex > 3)
        {
            currentRotationIndex = 0;
        }
    }

    private byte[,,] GetCurrentPieceDef()
    {
        if (currentPieceType == PieceType.I)
        {
            return pieceDefI;
        }
        else if (currentPieceType == PieceType.O)
        {
            return pieceDefO;
        }
        else if (currentPieceType == PieceType.J)
        {
            return pieceDefJ;
        }
        else if (currentPieceType == PieceType.L)
        {
            return pieceDefL;
        }
        else if (currentPieceType == PieceType.S)
        {
            return pieceDefS;
        }
        else if (currentPieceType == PieceType.Z)
        {
            return pieceDefZ;
        }
        else if (currentPieceType == PieceType.T)
        {
            return pieceDefT;
        }

        return pieceDefI;
    }

    public bool NextRotationHasBlockAt(int x, int y)
    {
        if (x < 0 || x > 4)
        {
            return false;
        }

        if (y < 0 || y > 4)
        {
            return false;
        }

        int testRotation = currentRotationIndex;
        testRotation++;
        if (testRotation > 3)
        {
            testRotation = 0;
        }

        if (GetCurrentPieceDef()[testRotation, x, y] != 0)
        {
            return true;
        }

        return false;        
    }

    public bool HasBlockAt(int x, int y)
    {
        if (x < 0 || x > 4)
        {
            return false;
        }

        if (y < 0 || y > 4)
        {
            return false;
        }

        if (GetCurrentPieceDef()[currentRotationIndex, x, y] != 0)
        {
            return true;
        }

        return false;
    }

    public BlockPosition GetPositionOfIndex(int index, bool withRotation)
    {
        if (index < 1 || index > 4)
        {
            return new BlockPosition(-1, -1);
        }

        int testRotation = currentRotationIndex;
        if (withRotation)
        {
            testRotation++;
            if (testRotation > 3)
            {
                testRotation = 0;
            }
        }

        for (int x = 0; x < 5; ++x)
        {
            for (int y = 0; y < 5; ++y)
            {
                if (GetCurrentPieceDef()[testRotation, x, y] == index)
                {
                    return new BlockPosition(x, y);
                }
            }
        }

        return new BlockPosition(-1, -1);
    }
}
