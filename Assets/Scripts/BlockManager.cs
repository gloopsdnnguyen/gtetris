using UnityEngine;
using System.Collections;

public class BlockPosition
{
    public int x;
    public int y;

    public BlockPosition(int setX, int setY)
    {
        x = setX;
        y = setY;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}


public class BlockManager : MonoBehaviour
{
    public GameObject BlockPrefab;
    public int MapX = 10;
    public int MapY = 20;
    public GUIStyle ScoreDisplayStyle;
    public GUIStyle GameOverStyle;
    public GUIStyle GameOverButtonStyle;
	public GUIStyle PreviewDisplayStyle;
    public float defaultDropTime=1.0f;
	
	public AudioClip GameOverSound;
	public AudioClip FlipSound;
	public AudioClip CompleteRowsSound;
	public AudioClip MoveSound;
	public AudioClip CollisionSound;
	
    private GameObject[,] stationaryBlocks;

    [HideInInspector] public Piece fallingPiece = null;
    private ArrayList fallingPieceGameObjects;
    private BlockPosition fallingPiecePosition;

    private float timeUntilNextDrop;

    private int score;
    private bool gameOver = false;
    public int currentLevel = 1;

    public float TimeBetweenHorizontalShift = 0.25f;
    private float timeSinceHorizontalShift = 0.0f;		
	
	private Piece nextPiece=null;
	private int difficulty_counter=0;
	private bool is_collision=false;
	
	private ArrayList clearPieceGameObjects;
	private ArrayList dropDownIndex;
	private bool locked=false;
	
    void Start()
    {
        score = 0;		
        timeUntilNextDrop = defaultDropTime;
        fallingPieceGameObjects = new ArrayList();
		clearPieceGameObjects   = new ArrayList();
		dropDownIndex  			= new ArrayList();
        stationaryBlocks = new GameObject[MapX, MapY];
        for (int x = 0; x < MapX; ++x)
        {
            for (int y = 0; y < MapY; ++y)
            {				
                stationaryBlocks[x, y] = null;			
            }
        }
        AddNewFallingPiece();
    }	
	
    private Vector3 Get3DPosition(BlockPosition position)
    {
        return new Vector3(position.x, position.y, 0.0f);
    }

    private GameObject GetBlockGameObjectAtPosition(BlockPosition position)
    {
		return stationaryBlocks[position.x, position.y];
    }

    private GameObject SetBlock(BlockPosition position)
    {
        GameObject newBlock = (GameObject)Instantiate(BlockPrefab, Get3DPosition(position), Quaternion.LookRotation(new Vector3(1.0f, 0.0f, 0.0f), Vector3.up));
		SetBlockColor(fallingPiece.currentPieceType,newBlock);
		GameObject oldGameObject = GetBlockGameObjectAtPosition(position);
        if (oldGameObject != null)
        {
            DestroyImmediate(oldGameObject);
        }
        stationaryBlocks[position.x, position.y] = newBlock;
        return newBlock;
    }
	
	private void SetBlockColor(Piece.PieceType currentPieceType,GameObject block){		

		if (currentPieceType == Piece.PieceType.I)
        {
			block.renderer.material.color=Color.red;
        }
        else if (currentPieceType == Piece.PieceType.O)
        {
			block.renderer.material.color=Color.cyan;
        }
        else if (currentPieceType == Piece.PieceType.J)
        {
			block.renderer.material.color=Color.magenta;
        }
        else if (currentPieceType == Piece.PieceType.L)
        {
			block.renderer.material.color=Color.yellow;
        }
        else if (currentPieceType == Piece.PieceType.S)
        {
			block.renderer.material.color=Color.blue;
        }
        else if (currentPieceType == Piece.PieceType.Z)
        {
			block.renderer.material.color=Color.green;
        }
        else if (currentPieceType == Piece.PieceType.T)
        {
			block.renderer.material.color=Color.grey;
        }
	}

    private void RemoveBlock(BlockPosition position)
    {
        if (stationaryBlocks[position.x, position.y] != null)
        {
            DestroyImmediate(stationaryBlocks[position.x, position.y]);
			stationaryBlocks[position.x, position.y]=null;
        }
    }

    private bool HasBlock(BlockPosition position)
    {
        if (stationaryBlocks[position.x, position.y] == null)
        {
            return false;
        }

        return true;
    }

    private void UpdateBlockPosition(BlockPosition position)
    {
        GameObject objToUpdate = GetBlockGameObjectAtPosition(position);
        if (objToUpdate != null)
        {
            objToUpdate.transform.position = Get3DPosition(position);
        }
    }

    private bool BlockPositionInMap(BlockPosition position)
    {
        if (position.x < 0 || position.x >= MapX)
        {
            return false;
        }
        else if (position.y < 0 || position.y >= MapY)
        {
            return false;
        }

        return true;
    }

    private void SwapBlocks(BlockPosition positionA, BlockPosition positionB)
    {
		
        GameObject tempObjectA = GetBlockGameObjectAtPosition(positionA);
        stationaryBlocks[positionA.x, positionA.y] = GetBlockGameObjectAtPosition(positionB);
        stationaryBlocks[positionB.x, positionB.y] = tempObjectA;
        UpdateBlockPosition(positionA);
        UpdateBlockPosition(positionB);
    }

    private void SwapPiece(BlockPosition oldPosition1, BlockPosition newPosition1, BlockPosition oldPosition2, BlockPosition newPosition2, BlockPosition oldPosition3, BlockPosition newPosition3, BlockPosition oldPosition4, BlockPosition newPosition4)
    {
        GameObject tempObject1 = GetBlockGameObjectAtPosition(oldPosition1);
        GameObject tempObject2 = GetBlockGameObjectAtPosition(oldPosition2);
        GameObject tempObject3 = GetBlockGameObjectAtPosition(oldPosition3);
        GameObject tempObject4 = GetBlockGameObjectAtPosition(oldPosition4);

        stationaryBlocks[oldPosition1.x, oldPosition1.y] = null;
        stationaryBlocks[oldPosition2.x, oldPosition2.y] = null;
        stationaryBlocks[oldPosition3.x, oldPosition3.y] = null;
        stationaryBlocks[oldPosition4.x, oldPosition4.y] = null;

        stationaryBlocks[newPosition1.x, newPosition1.y] = tempObject1;
        stationaryBlocks[newPosition2.x, newPosition2.y] = tempObject2;
        stationaryBlocks[newPosition3.x, newPosition3.y] = tempObject3;
        stationaryBlocks[newPosition4.x, newPosition4.y] = tempObject4;

        UpdateBlockPosition(oldPosition1);
        UpdateBlockPosition(oldPosition2);
        UpdateBlockPosition(oldPosition3);
        UpdateBlockPosition(oldPosition4);
        UpdateBlockPosition(newPosition1);
        UpdateBlockPosition(newPosition2);
        UpdateBlockPosition(newPosition3);
        UpdateBlockPosition(newPosition4);
    }

    private BlockPosition GetPositionFromGameObject(GameObject blockObject)
    {
        for (int x = 0; x < MapX; ++x)
        {
            for (int y = 0; y < MapY; ++y)
            {
                if (stationaryBlocks[x, y] == blockObject)
                {
                    return new BlockPosition(x, y);
                }
            }
        }

        return new BlockPosition(-1, -1);
    }   

    private BlockPosition GetFallingPieceStartPosition()
    {		
        return new BlockPosition((int)(MapX / 2) - 3, MapY-5);		
    }

    private void AddFallingPiecesToStationary()
    {
        fallingPiece = null;
        fallingPieceGameObjects.Clear();
    }

    private bool FallingPieceFits(BlockPosition testPosition, bool tryRotate)
    {
        for (int x = 0; x < 5; ++x)
        {
            for (int y = 0; y < 5; ++y)
            {
                bool fallingPieceHasBlock = false;
                if (tryRotate == false && fallingPiece.HasBlockAt(x, y))
                {
                    fallingPieceHasBlock = true;
                }
                else if (tryRotate == true && fallingPiece.NextRotationHasBlockAt(x, y))
                {
                    fallingPieceHasBlock = true;
                }

                if (fallingPieceHasBlock == true)
                {
                    if (BlockPositionInMap(new BlockPosition(testPosition.x + x, testPosition.y + y)) == false)
                    {
                        return false;
                    }

                    GameObject blockingGameObject = GetBlockGameObjectAtPosition(new BlockPosition(testPosition.x + x, testPosition.y + y));
                    if (blockingGameObject != null && fallingPieceGameObjects.Contains(blockingGameObject) == false)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void SetFallingPieceBlocks()
    {
		if(is_collision){
			audio.PlayOneShot(CollisionSound);
		}
        for (int x = 0; x < 5; ++x)
        {
            for (int y = 0; y < 5; ++y)
            {
                if (fallingPiece.HasBlockAt(x, y) == true)
                {
                    GameObject newBlock = SetBlock(new BlockPosition(fallingPiecePosition.x + x, fallingPiecePosition.y + y));					
                    fallingPieceGameObjects.Add(newBlock);
                }
            }
        }
		is_collision=true;
    }
	
	private void SetPreviewPieceBlocks()
    {
        for (int x = 0; x < 5; ++x)
        {
            for (int y = 0; y < 5; ++y)
            {
                if (nextPiece.HasBlockAt(x, y) == true)
                {
                    SetPreviewBlock(new BlockPosition((int)(MapX / 2) +7+ x, MapY-6+y));
                }
            }
        }
    }
	
	private void SetPreviewBlock(BlockPosition position)
    {
		GameObject previewBlock = (GameObject)Instantiate(BlockPrefab, Get3DPosition(position), Quaternion.LookRotation(new Vector3(1.0f, 0.0f, 0.0f), Vector3.up));
        previewBlock.tag="Preview";	
		SetBlockColor(nextPiece.currentPieceType,previewBlock);
    }
	

    private void AddNewFallingPiece()
    {
        if (fallingPiece != null)
        {
            return;
        }		
		if(nextPiece==null){
			fallingPiece = new Piece();
			nextPiece=new Piece();
		}else{
			fallingPiece=nextPiece;
			nextPiece = new Piece();
		}
        fallingPiecePosition = GetFallingPieceStartPosition();
		
        if (FallingPieceFits(fallingPiecePosition, false) == false)
        {
            gameOver = true;
            FillMapWithRandomBlocks();
            return;
        }	        
		SetPreviewPieceBlocks();
        SetFallingPieceBlocks();
    }

    private void FillMapWithRandomBlocks()
    {
        for (int x = 0; x < MapX; ++x)
        {
            for (int y = 0; y < MapY; ++y)
            {
                if (stationaryBlocks[x, y] == null)
                {
                    SetBlock(new BlockPosition(x, y));
                }
            }
        }
    }

    private void DropFallingPiece()
    {
        if (FallingPieceFits(new BlockPosition(fallingPiecePosition.x, fallingPiecePosition.y - 1), false))
        {
            for (int y = 0; y < 5; ++y)
            {
                for (int x = 0; x < 5; ++x)
                {
                    if (fallingPiece.HasBlockAt(x, y) == true)
                    {
                        BlockPosition originalPosition = new BlockPosition(fallingPiecePosition.x + x, fallingPiecePosition.y + y);
                        BlockPosition newPosition = new BlockPosition(fallingPiecePosition.x + x, fallingPiecePosition.y + y - 1);

                        if (BlockPositionInMap(originalPosition) && BlockPositionInMap(newPosition))
                        {
                            SwapBlocks(originalPosition, newPosition);
                        }
                    }
                }
            }
            fallingPiecePosition.y -= 1;
        }
        else
        {
            AddFallingPiecesToStationary();	
			GameObject[] previews = GameObject.FindGameObjectsWithTag("Preview");
		    foreach(GameObject p in previews){
		        DestroyImmediate(p);
			}			
            AddNewFallingPiece();
        }
    }

    private void RotateFallingPiece()
    {
        if (FallingPieceFits(fallingPiecePosition, true) == true)
        {
            BlockPosition oldPosition1 = fallingPiece.GetPositionOfIndex(1, false);
            BlockPosition oldPosition2 = fallingPiece.GetPositionOfIndex(2, false);
            BlockPosition oldPosition3 = fallingPiece.GetPositionOfIndex(3, false);
            BlockPosition oldPosition4 = fallingPiece.GetPositionOfIndex(4, false);

            BlockPosition newPosition1 = fallingPiece.GetPositionOfIndex(1, true);
            BlockPosition newPosition2 = fallingPiece.GetPositionOfIndex(2, true);
            BlockPosition newPosition3 = fallingPiece.GetPositionOfIndex(3, true);
            BlockPosition newPosition4 = fallingPiece.GetPositionOfIndex(4, true);

            BlockPosition totalOldPosition1 = new BlockPosition(fallingPiecePosition.x + oldPosition1.x, fallingPiecePosition.y + oldPosition1.y);
            BlockPosition totalOldPosition2 = new BlockPosition(fallingPiecePosition.x + oldPosition2.x, fallingPiecePosition.y + oldPosition2.y);
            BlockPosition totalOldPosition3 = new BlockPosition(fallingPiecePosition.x + oldPosition3.x, fallingPiecePosition.y + oldPosition3.y);
            BlockPosition totalOldPosition4 = new BlockPosition(fallingPiecePosition.x + oldPosition4.x, fallingPiecePosition.y + oldPosition4.y);

            BlockPosition totalNewPosition1 = new BlockPosition(fallingPiecePosition.x + newPosition1.x, fallingPiecePosition.y + newPosition1.y);
            BlockPosition totalNewPosition2 = new BlockPosition(fallingPiecePosition.x + newPosition2.x, fallingPiecePosition.y + newPosition2.y);
            BlockPosition totalNewPosition3 = new BlockPosition(fallingPiecePosition.x + newPosition3.x, fallingPiecePosition.y + newPosition3.y);
            BlockPosition totalNewPosition4 = new BlockPosition(fallingPiecePosition.x + newPosition4.x, fallingPiecePosition.y + newPosition4.y);

            SwapPiece(totalOldPosition1, totalNewPosition1, totalOldPosition2, totalNewPosition2, totalOldPosition3, totalNewPosition3, totalOldPosition4, totalNewPosition4);
			audio.PlayOneShot(FlipSound);
            fallingPiece.IncrementRotation();
        }
    }

    private void MoveFallingPieceHorizontal(bool moveLeft)
    {
        if (moveLeft)
        {
            if (FallingPieceFits(new BlockPosition(fallingPiecePosition.x - 1, fallingPiecePosition.y), false) == true)
            {
                for (int x = 0; x < 5; ++x)
                {
                    for (int y = 0; y < 5; ++y)
                    {
                        if (fallingPiece.HasBlockAt(x, y) == true)
                        {
                            BlockPosition originalPosition = new BlockPosition(fallingPiecePosition.x + x, fallingPiecePosition.y + y);
                            BlockPosition newPosition = new BlockPosition(fallingPiecePosition.x + x - 1, fallingPiecePosition.y + y);

                            if (BlockPositionInMap(originalPosition) && BlockPositionInMap(newPosition))
                            {
                                SwapBlocks(originalPosition, newPosition);
                            }
                        }
                    }
                }
                fallingPiecePosition.x -= 1;
            }
        }
        else
        {
            if (FallingPieceFits(new BlockPosition(fallingPiecePosition.x + 1, fallingPiecePosition.y), false) == true)
            {
                for (int x = 4; x >= 0; --x)
                {
                    for (int y = 0; y < 5; ++y)
                    {
                        if (fallingPiece.HasBlockAt(x, y) == true)
                        {
                            BlockPosition originalPosition = new BlockPosition(fallingPiecePosition.x + x, fallingPiecePosition.y + y);
                            BlockPosition newPosition = new BlockPosition(fallingPiecePosition.x + x + 1, fallingPiecePosition.y + y);

                            if (BlockPositionInMap(originalPosition) && BlockPositionInMap(newPosition))
                            {
                                SwapBlocks(originalPosition, newPosition);
                            }
                        }
                    }
                }
                fallingPiecePosition.x += 1;
            }
        }
    }    

    void OnGUI()
    {
        GUI.Label(screenRect(20.0f, 20.0f, 500.0f, 50.0f), "Score: " + score, ScoreDisplayStyle);
		GUI.Label(screenRect(20.0f, 50.0f, 500.0f, 50.0f), "Level: " + currentLevel, ScoreDisplayStyle);		
		
		/*
		if (GUI.Button(screenRect(20.0f, 400.0f, 100.0f, 100.0f), "Flip")&& fallingPiece != null){		
            RotateFallingPiece();        
		}
		if (GUI.RepeatButton(screenRect(130.0f, 400.0f, 100.0f, 100.0f), "Down")&& fallingPiece != null){		
            timeUntilNextDrop -= Time.deltaTime * 15.0f;       
		}		
		if (GUI.RepeatButton(screenRect(540.0f, 400.0f, 100.0f, 100.0f), "Left")&& fallingPiece != null && timeSinceHorizontalShift > TimeBetweenHorizontalShift){		
             MoveFallingPieceHorizontal(true);
             timeSinceHorizontalShift = 0.0f;        
		}
		if (GUI.RepeatButton(screenRect(650.0f, 400.0f, 100.0f, 100.0f), "Right")&& fallingPiece != null && timeSinceHorizontalShift > TimeBetweenHorizontalShift){		
             MoveFallingPieceHorizontal(false);
             timeSinceHorizontalShift = 0.0f;     
		}*/	
		if (GUI.Button(screenRect(540.0f, 400.0f, 100.0f, 100.0f), "Flip")&& fallingPiece != null){		
            RotateFallingPiece();        
		}
		if (GUI.RepeatButton(screenRect(130.0f, 400.0f, 100.0f, 100.0f), "Down")&& fallingPiece != null){	
            timeUntilNextDrop -= Time.deltaTime * 15.0f;       
		}		
		if (GUI.RepeatButton(screenRect(20.0f, 400.0f, 100.0f, 100.0f), "Left")&& fallingPiece != null && timeSinceHorizontalShift > TimeBetweenHorizontalShift){		
             MoveFallingPieceHorizontal(true);
             timeSinceHorizontalShift = 0.0f;        
		}
		if (GUI.RepeatButton(screenRect(650.0f, 400.0f, 100.0f, 100.0f), "Right")&& fallingPiece != null && timeSinceHorizontalShift > TimeBetweenHorizontalShift){		
             MoveFallingPieceHorizontal(false);
             timeSinceHorizontalShift = 0.0f;     
		}
		
        if (gameOver == true)
        {
			audio.PlayOneShot(GameOverSound);
            GUI.Box(new Rect(Screen.width / 3.0f, Screen.height / 4.0f, Screen.width / 3.5f, Screen.height / 2.0f), "Game Over!\n\nFinal Score: " + score, GameOverStyle);
            if (GUI.Button(new Rect(Screen.width / 2.5f, Screen.height / 2.0f, Screen.width / 8.0f, Screen.height / 8.0f), "Restart", GameOverButtonStyle))
            {
                Application.LoadLevel(0);
            }
        }
    }
    

    void Update()
    {
		if(!locked){
        	if (gameOver == true)
	        {
	            return;
	        }
			
			int point = CheckCompletedRows();
			UpdateScore(point);		
			
		       
	        timeSinceHorizontalShift += Time.deltaTime;
	
	        timeUntilNextDrop -= Time.deltaTime;
	        if (timeUntilNextDrop < 0.0f)
	        {
	            DropFallingPiece();			
	            timeUntilNextDrop = defaultDropTime - Mathf.Lerp(0.0f, 1.0f, (float)currentLevel / 30.0f);
	        }
	
	        if (Input.GetKey(KeyCode.LeftArrow) && fallingPiece != null && timeSinceHorizontalShift > TimeBetweenHorizontalShift)
	        {
	            MoveFallingPieceHorizontal(true);
	            timeSinceHorizontalShift = 0.0f;
	        }
	        else if (Input.GetKey(KeyCode.RightArrow) && fallingPiece != null && timeSinceHorizontalShift > TimeBetweenHorizontalShift)
	        {
	            MoveFallingPieceHorizontal(false);
	            timeSinceHorizontalShift = 0.0f;
	        }
	        else if (Input.GetKeyDown(KeyCode.W) && fallingPiece != null)
	        {
	            RotateFallingPiece();
	        }
	        else if (Input.GetKey(KeyCode.DownArrow) && fallingPiece != null)
	        {
	            timeUntilNextDrop -= Time.deltaTime * 15.0f;
	        }		
		}
    }
	
	private void ClearRow(int y)
	{		
		
		for(int i = 0;i<MapX;i++)
		{
			RemoveBlock(new BlockPosition(i, y));
		}
	}	
	
	private void AddClearBlocks(int y){		
		for(int i = 0;i<MapX;i++)
		{			
			clearPieceGameObjects.Add(new BlockPosition(i, y));
		}		
	}
	
	private void AddDropDownIndex(int y){		
		dropDownIndex.Add(y);		
	}
	
	
	private bool RowIsComplete(int y)
	{
		for(int i = 0;i<MapX;i++)
		{
			if(BlockIsEmpty(i,y))
			{
				return false;
			}
		}		
		return true;
	}	
	
	private int CheckCompletedRows()
	{
		int num_completed_rows = 0;
		for(int i = 0;i<MapY;i++)
		{
			if(RowIsComplete(i))
			{									
				is_collision=false;
				AddClearBlocks(i);
				AddDropDownIndex(i+1);
				//ClearRow(i);
				//DropAllHigherRows(i+1);
				num_completed_rows++;
			}		
		}				
		return num_completed_rows;
	}
	
	private void DropAllHigherRows(int y)
	{
		
		for(int i = y;i<MapY;i++)
		{
			DropRow(i, 1);
		}
	}
	
	private void DropRow(int y, int num_units)
	{
		for(int i = 0;i<MapX;i++)
		{
			SwapBlocks(new BlockPosition(i, y), new BlockPosition(i, y-num_units));
		}
	}
	
	private void UpdateScore(int rows)
    {
		//ClearRow(i);
		//DropAllHigherRows(i+1);		
		if( clearPieceGameObjects.Count>0){
			Color tColor=Color.white;
			foreach(BlockPosition bp in clearPieceGameObjects){
				GameObject tempGo = GetBlockGameObjectAtPosition(bp);
				tColor = tempGo.renderer.material.color;
				tempGo.renderer.material.color= Color.white;
			}			
			audio.PlayOneShot(CompleteRowsSound);
		    StartCoroutine(Wait(tColor));			
		}
		
		score += rows*100;
		if(rows>=4){//tetris
			score+=200;
		}	
		difficulty_counter += rows;
		if(difficulty_counter>10)
		{
			difficulty_counter -= 10;
			currentLevel++;
		}
		if (currentLevel > 29)
        {
            currentLevel = 29;
        }
    }
	
	private IEnumerator Wait(Color t_color) {
		locked=true;		
		foreach(BlockPosition bp in clearPieceGameObjects){
			GameObject tempGo = GetBlockGameObjectAtPosition(bp);
			tempGo.renderer.material.color= t_color;
		}
    	yield return new WaitForSeconds(0.125f);
		foreach(BlockPosition bp in clearPieceGameObjects){
			GameObject tempGo1 = GetBlockGameObjectAtPosition(bp);
			tempGo1.renderer.material.color= Color.white;
		}
		yield return new WaitForSeconds(0.125f);
		locked=false;
		foreach(BlockPosition bp in clearPieceGameObjects){
			RemoveBlock(bp);
		}
		clearPieceGameObjects.Clear();
		if(dropDownIndex.Count>0){
			foreach(int i in dropDownIndex){
				DropAllHigherRows(i);
			}
			dropDownIndex.Clear();
		}
	}
	
	private bool BlockIsEmpty(int x, int y)
	{
		if(x<0||x>=MapX||y<0||y>=MapY)
		{
			return false;
		}
		if (stationaryBlocks[x, y] == null)
        {
            return true;
        }
        return false;
	}
	
	private static Rect screenRect(float tx, float ty,float tw, float th) 
    {
               //800x480 is standard screen size
       float x1 = tx * Screen.width/800;
       float y1 = ty * Screen.height/480;
       
       float sw = tw * Screen.width/800;
       float sh = th * Screen.height/480;
               
       return new Rect(x1,y1,sw,sh);
    }
	
}
