using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    //number of intersections generated off main branch
    private float generatedLoops = 0;
    
    //soft cap to intersections generated off main branch
    public int roomNumberSoftCap;
      //min space between rooms
    public float hallwayOffset = 10;
    //cap on main path rooms
    public float firstBatchRooms = 9;
    public float maxIterations = 100;
    private float iteration = 0;
    public bool disaplyMainPath = false;
    public GameObject wall;
    public Material mainPathMaterial;
    public SplineGenerator splineGenerator;
    public List<GameObject> oneDoorRooms;
    public List<GameObject> twoDoorRooms;

    public List<GameObject> rareRooms;
    public List<GameObject> fourDoorRooms;

    public List<GameObject> essentialRooms;

    public GameObject startRoom;
    
    public GameObject endRoom;

    public bool mapBuilt = false;

    public GameObject level;
  
    //used for iteration
    private GameObject previousRoomObject;
    //list of paired doorways to send to hallway gen
    List<Transform> doorways = new List<Transform>();
    //list of all rooms generated
    public List<GameObject> rooms = new List<GameObject>();
    //list of rooms generated in a loop that can not be added until loop end due to c# limitations
    public List<GameObject> roomsToAddOnLoopEnd = new List<GameObject>();



    // Start is called before the first frame update
    
    public void buildMap(){
        if(mapBuilt == false){
            mapBuilt = true;
          //place start room at origin
        previousRoomObject = Instantiate(startRoom, startRoom.transform.position, quaternion.identity);
        rooms.Add(previousRoomObject);
       //dosen't check for collision
       //if collision remove door and wall off, choose another doorway
        generateMainBatch();
        generateEndRoom();
        

        generateBranches();
        generateEssential();
        generatedEndCaps();
        


       
        splineGenerator.generateHallways(doorways);
        GameObject player = GameObject.Find("Player");
        GameObject level = new GameObject("Level");
        player.GetComponent<CharacterController>().enabled = false;
        GameObject.Find("Player").transform.position = new Vector3(0,10,0);
        player.GetComponent<CharacterController>().enabled = true;
        //parent rooms to level
        foreach(GameObject room in rooms){
            room.transform.parent = level.transform;
        }
       

    }}

    // Update is called once per frame
    //takes a door from the previous room and generates a new room of it x times then places the end room
    public void generateMainBatch(){
        for(int i = 0; i< firstBatchRooms; i++){
        GameObject chosenRoomObject = null;
        Room previousRoom = previousRoomObject.GetComponent<Room>();
        Vector3 previousRoomPostion = previousRoomObject.transform.position;
        //select random door from previous room
        int chosenDoorwayIndex = UnityEngine.Random.Range(0,previousRoom.GetComponent<Room>().doorways.Count);
        Transform startDoorway = previousRoom.doorways[chosenDoorwayIndex];
        int roomType = 0;
        int roomIndex = 0;
        //choose room type
        if(i == 0 || i == 3){
            roomType = 1;
        }else{
        roomType = UnityEngine.Random.Range(0,2);
        }
        
       switch(roomType){
        //two door rooms
        case 0:
        roomIndex = UnityEngine.Random.Range(0,twoDoorRooms.Count);
        chosenRoomObject = twoDoorRooms[roomIndex];
        break;

        //three door rooms
        case 1:
        roomIndex = UnityEngine.Random.Range(0,rareRooms.Count);
        chosenRoomObject = rareRooms[roomIndex];
        break;
       }

        Room chosenRoom = chosenRoomObject.GetComponent<Room>();

        //choose door from chosenRoom
        chosenDoorwayIndex = UnityEngine.Random.Range(0,chosenRoom.doorways.Count);
        Transform chosenDoorway = chosenRoom.doorways[chosenDoorwayIndex];

        //get global rotations of both doorways

        float chosenDoorwayRotation = chosenDoorway.eulerAngles.y;
        float startDoorwayRotation = startDoorway.eulerAngles.y;
    

        float rotationOffset =  180 - chosenDoorwayRotation + startDoorwayRotation;
       
        //move room by an offset + half the width of both rooms
        float previousRoomSize = previousRoom.GetComponent<BoxCollider>().size.z/2 + previousRoom.GetComponent<BoxCollider>().size.x/2;
        float chosenRoomSize = chosenRoomObject.GetComponent<BoxCollider>().size.z/2 + chosenRoomObject.GetComponent<BoxCollider>().size.x/2;
        float roomOffset = chosenRoomSize + previousRoomSize + hallwayOffset;
        

    
        Vector3 directionToMove = startDoorway.forward;
        Vector3 amountToMove = directionToMove*roomOffset + previousRoomPostion;
        

        //place the room

        GameObject generatedRoom = Instantiate(chosenRoomObject, 
        new Vector3(amountToMove.x, chosenRoomObject.transform.position.y, amountToMove.z),
        Quaternion.Euler(new Vector3(0,rotationOffset,0)));
        
     bool roomColliding = false;
        foreach(GameObject room in rooms){
           
             if(generatedRoom.GetComponent<BoxCollider>().bounds.Intersects(room.GetComponent<BoxCollider>().bounds)){
                roomColliding = true;

                
            }
        }

         foreach(GameObject room in roomsToAddOnLoopEnd){
           
             if(generatedRoom.GetComponent<BoxCollider>().bounds.Intersects(room.GetComponent<BoxCollider>().bounds)){
                roomColliding = true;

                
            }
        }
       
        if(roomColliding){
            DestroyImmediate(generatedRoom.gameObject);
            i = i-1;
            
        }else{
        //doorways to list;
        Transform generatedDoorway = generatedRoom.GetComponent<Room>().doorways[chosenDoorwayIndex];
        doorways.Add(startDoorway);
        doorways.Add(generatedDoorway);
        //remove doorways from their rooms

        generatedRoom.GetComponent<Room>().doorways.Remove(generatedDoorway);
        previousRoom.doorways.Remove(startDoorway);
        rooms.Add(generatedRoom);
        previousRoomObject = generatedRoom;
        if(disaplyMainPath){
        generatedRoom.GetComponentInChildren<MeshRenderer>().material = mainPathMaterial;
        }

        }


    }}

    public void generateEndRoom(){
         GameObject chosenRoomObject = null;
        Room previousRoom = previousRoomObject.GetComponent<Room>();
        Vector3 previousRoomPostion = previousRoomObject.transform.position;
        //select random door from previous room
        int chosenDoorwayIndex = UnityEngine.Random.Range(0,previousRoom.GetComponent<Room>().doorways.Count);
        Transform startDoorway = previousRoom.doorways[chosenDoorwayIndex];

        //choose room type
       
        chosenRoomObject = endRoom;

        Room chosenRoom = chosenRoomObject.GetComponent<Room>();

        //choose door from chosenRoom
        chosenDoorwayIndex = UnityEngine.Random.Range(0,chosenRoom.doorways.Count);
        Transform chosenDoorway = chosenRoom.doorways[chosenDoorwayIndex];

        //get global rotations of both doorways

        float chosenDoorwayRotation = chosenDoorway.eulerAngles.y;
        float startDoorwayRotation = startDoorway.eulerAngles.y;
    

        float rotationOffset =  180 - chosenDoorwayRotation + startDoorwayRotation;
       

       
        //move room by an offset + half the width of both rooms
        float previousRoomSize = previousRoom.GetComponent<BoxCollider>().size.z/2 + previousRoom.GetComponent<BoxCollider>().size.x/2;
        float chosenRoomSize = chosenRoomObject.GetComponent<BoxCollider>().size.z/2 + chosenRoomObject.GetComponent<BoxCollider>().size.x/2;
        float roomOffset = chosenRoomSize + previousRoomSize + hallwayOffset;
        

    
        Vector3 directionToMove = startDoorway.forward;
        Vector3 amountToMove = directionToMove*roomOffset + previousRoomPostion;
        

        //place the room

        GameObject generatedRoom = Instantiate(chosenRoomObject, 
        new Vector3(amountToMove.x, chosenRoomObject.transform.position.y, amountToMove.z),
        Quaternion.Euler(new Vector3(0,rotationOffset,0)));
        

        //doorways to list;
        Transform generatedDoorway = generatedRoom.GetComponent<Room>().doorways[chosenDoorwayIndex];
        doorways.Add(startDoorway);
        doorways.Add(generatedDoorway);
        //remove doorways from their rooms

        generatedRoom.GetComponent<Room>().doorways.Remove(generatedDoorway);
        previousRoom.doorways.Remove(startDoorway);
        previousRoomObject = generatedRoom;
        rooms.Add(generatedRoom);

        }


    public void generateBranch(){
        //loop through rooms until a room is found with a door
      
        foreach(GameObject roomObject in rooms){
            if(roomObject.GetComponent<Room>().doorways.Count > 0){
                //choose a doorway, make a room
                Transform startDoorway = roomObject.GetComponent<Room>().doorways[0];
                GameObject previousRoomObject = roomObject;
            
        
        
        
        
        
        
        GameObject chosenRoomObject = null;
        Room previousRoom = previousRoomObject.GetComponent<Room>();
        Vector3 previousRoomPostion = previousRoomObject.transform.position;
        //select random door from previous room

        //choose room type
        int roomType = 0;
        
         roomType = UnityEngine.Random.Range(0,2);
     
        
        int roomIndex = 0;
       switch(roomType){
        //two door rooms
        case 0:
        roomIndex = UnityEngine.Random.Range(0,twoDoorRooms.Count);
        chosenRoomObject = twoDoorRooms[roomIndex];
        break;

        //three door rooms
        case 1:
        roomIndex = UnityEngine.Random.Range(0,rareRooms.Count);
        chosenRoomObject = rareRooms[roomIndex];
        break;

       }


        Room chosenRoom = chosenRoomObject.GetComponent<Room>();

        //choose door from chosenRoom
        int chosenDoorwayIndex = UnityEngine.Random.Range(0,chosenRoom.doorways.Count);
        Transform chosenDoorway = chosenRoom.doorways[chosenDoorwayIndex];

        //get global rotations of both doorways

        float chosenDoorwayRotation = chosenDoorway.eulerAngles.y;
        float startDoorwayRotation = startDoorway.eulerAngles.y;
    

        float rotationOffset =  180 - chosenDoorwayRotation + startDoorwayRotation;
       

       
        //move room by an offset + half the width of both rooms
        float previousRoomSize = previousRoom.GetComponent<BoxCollider>().size.z/2 + previousRoom.GetComponent<BoxCollider>().size.x/2;
        float chosenRoomSize = chosenRoomObject.GetComponent<BoxCollider>().size.z/2 + chosenRoomObject.GetComponent<BoxCollider>().size.x/2;
        float roomOffset = chosenRoomSize + previousRoomSize + hallwayOffset;
        

    
        Vector3 directionToMove = startDoorway.forward;
        Vector3 amountToMove = directionToMove*roomOffset + previousRoomPostion;
        

        //place the room

        GameObject generatedRoom = Instantiate(chosenRoomObject, 
        new Vector3(amountToMove.x, chosenRoomObject.transform.position.y, amountToMove.z),
        Quaternion.Euler(new Vector3(0,rotationOffset,0)));


        //CHECK OVERLAP WITH OTHER ROOM
        //IF OVERLAP,REMOVE DOOR FROM LIST, DELETE ROOM,WALL OFF
        bool roomColliding = false;
        foreach(GameObject room in rooms){
           
             if(generatedRoom.GetComponent<BoxCollider>().bounds.Intersects(room.GetComponent<BoxCollider>().bounds)){
                roomColliding = true;

                
            }
        }

         foreach(GameObject room in roomsToAddOnLoopEnd){
           
             if(generatedRoom.GetComponent<BoxCollider>().bounds.Intersects(room.GetComponent<BoxCollider>().bounds)){
                roomColliding = true;

                
            }
        }
       
        if(roomColliding){
            DestroyImmediate(generatedRoom.gameObject);
            previousRoom.doorways.Remove(startDoorway);
            //wall of door
            Instantiate(wall,startDoorway.position,quaternion.identity).transform.parent = level.transform;

            
        }else{
       




        

        //doorways to list;
        Transform generatedDoorway = generatedRoom.GetComponent<Room>().doorways[chosenDoorwayIndex];
        doorways.Add(startDoorway);
        doorways.Add(generatedDoorway);
        //remove doorways from their rooms

        generatedRoom.GetComponent<Room>().doorways.Remove(generatedDoorway);
        previousRoom.doorways.Remove(startDoorway);
        roomsToAddOnLoopEnd.Add(generatedRoom);
        generatedLoops += 1;
        }

        }}

        rooms.AddRange(roomsToAddOnLoopEnd);
        roomsToAddOnLoopEnd = new List<GameObject>();
            



        }

    public void generateBranches(){
        bool keepGenerating = true;
       
        List<Transform> Remainingdoorways = new List<Transform>();
        while(keepGenerating){
            
            Remainingdoorways = new List<Transform>();
            foreach(GameObject roomObject in rooms){
             Remainingdoorways.AddRange(roomObject.GetComponent<Room>().doorways);
            }
            
            if(Remainingdoorways.Count > 0){
                keepGenerating = true; 
                generateBranch();

            }else{
                keepGenerating = false;
            }
            if(generatedLoops > roomNumberSoftCap){
                keepGenerating = false;
            }


        }
        Debug.Log(generatedLoops);




    }
   

   
    public void generateEssential(){
        ///IF ONE IS FAILED TO PLACE IT WILL NOT BE PLACED AT ALL :(
        //loop over rooms that must be added
        //currently possible to loop infinitly
        foreach(GameObject essentialRoom in essentialRooms){
            bool failedToPlaceEssential = true;
            GameObject chosenRoomObject = essentialRoom;
            Room chosenRoom = essentialRoom.GetComponent<Room>();

            //choose door from chosenRoom
             int chosenDoorwayIndex = UnityEngine.Random.Range(0,chosenRoom.doorways.Count);
            Transform chosenDoorway = chosenRoom.doorways[chosenDoorwayIndex];
        iteration = 0;
        while(failedToPlaceEssential == true &&  iteration < maxIterations){
            iteration++;
            //choose random room, choose random doorway from that room
            
            List<GameObject> remainingRoomsWithDoors = new List<GameObject>();
            //get remaining doorways
            foreach(GameObject room in rooms){
                if(room.GetComponent<Room>().doorways.Count > 0){
                    remainingRoomsWithDoors.Add(room);
                }
            }
            //choose a random doorway from the list
            //check if there are still doorways left?
            int startRoomIndex = UnityEngine.Random.Range(0,remainingRoomsWithDoors.Count);
            previousRoomObject = remainingRoomsWithDoors[startRoomIndex];
            Vector3 previousRoomPostion = previousRoomObject.transform.position;

            int startDoorwayIndex = UnityEngine.Random.Range(0,previousRoomObject.GetComponent<Room>().doorways.Count);
            Transform startDoorway = previousRoomObject.GetComponent<Room>().doorways[startDoorwayIndex];
            

        

        //get global rotations of both doorways

        float chosenDoorwayRotation = chosenDoorway.eulerAngles.y;
        float startDoorwayRotation = startDoorway.eulerAngles.y;
    

        float rotationOffset =  180 - chosenDoorwayRotation + startDoorwayRotation;
       

       
        //move room by an offset + half the width of both rooms
        
        float chosenRoomSize = chosenRoomObject.GetComponent<BoxCollider>().size.z/2 + chosenRoomObject.GetComponent<BoxCollider>().size.x/2;
        float previousRoomSize = previousRoomObject.GetComponent<BoxCollider>().size.z/2 + previousRoomObject.GetComponent<BoxCollider>().size.x/2;
        float roomOffset = chosenRoomSize + hallwayOffset + previousRoomSize;
        

    
        Vector3 directionToMove = startDoorway.forward;
        Vector3 amountToMove = directionToMove*roomOffset + previousRoomPostion;
        

        //place the room

        GameObject generatedRoom = Instantiate(chosenRoomObject, 
        new Vector3(amountToMove.x, chosenRoomObject.transform.position.y, amountToMove.z),
        Quaternion.Euler(new Vector3(0,rotationOffset,0)));





        //CHECK OVERLAP WITH OTHER ROOM
        //IF OVERLAP,REMOVE DOOR FROM LIST, DELETE ROOM,WALL OFF
        bool roomColliding = false;
        
        foreach(GameObject room in rooms){

             if(generatedRoom.GetComponent<BoxCollider>().bounds.Intersects(room.GetComponent<BoxCollider>().bounds)){
                roomColliding = true;

                
            }
        }

        
       
        if(roomColliding){
            DestroyImmediate(generatedRoom.gameObject);
            //previousRoomObject.GetComponent<Room>().doorways.Remove(startDoorway);
            //wall of door
            //Instantiate(wall,startDoorway.position,quaternion.identity);
            failedToPlaceEssential = true;
        }else{
        failedToPlaceEssential = false;
        

        //doorways to list;
        Transform generatedDoorway = generatedRoom.GetComponent<Room>().doorways[chosenDoorwayIndex];
        doorways.Add(startDoorway);
        doorways.Add(generatedDoorway);
        //remove doorways from their rooms

        generatedRoom.GetComponent<Room>().doorways.Remove(generatedDoorway);
        previousRoomObject.GetComponent<Room>().doorways.Remove(startDoorway);
        rooms.Add(generatedRoom);
       
        }

        }}}

     

public void generatedEndCaps(){
    bool keepGenerating = true;
       
        List<Transform> Remainingdoorways = new List<Transform>();
        while(keepGenerating){
            
            Remainingdoorways = new List<Transform>();
            foreach(GameObject roomObject in rooms){
             Remainingdoorways.AddRange(roomObject.GetComponent<Room>().doorways);
            }
            
            if(Remainingdoorways.Count > 0){
                keepGenerating = true; 
                generateEndCap();

            }else{
                keepGenerating = false;
            }






}
}

public void generateEndCap(){
foreach(GameObject roomObject in rooms){
            if(roomObject.GetComponent<Room>().doorways.Count > 0){
                //choose a doorway, make a room
                Transform startDoorway = roomObject.GetComponent<Room>().doorways[0];
                GameObject previousRoomObject = roomObject;
            
        
        
        
        
        
        
        GameObject chosenRoomObject = null;
        Room previousRoom = previousRoomObject.GetComponent<Room>();
        Vector3 previousRoomPostion = previousRoomObject.transform.position;
        //select random door from previous room

       
     
        
       int roomIndex = UnityEngine.Random.Range(0,oneDoorRooms.Count);
       chosenRoomObject = oneDoorRooms[roomIndex];


        Room chosenRoom = chosenRoomObject.GetComponent<Room>();

        //choose door from chosenRoom
        int chosenDoorwayIndex = UnityEngine.Random.Range(0,chosenRoom.doorways.Count);
        Transform chosenDoorway = chosenRoom.doorways[chosenDoorwayIndex];

        //get global rotations of both doorways

        float chosenDoorwayRotation = chosenDoorway.eulerAngles.y;
        float startDoorwayRotation = startDoorway.eulerAngles.y;
    

        float rotationOffset =  180 - chosenDoorwayRotation + startDoorwayRotation;
       

       
        //move room by an offset + half the width of both rooms
        float previousRoomSize = previousRoom.GetComponent<BoxCollider>().size.z/2 + previousRoom.GetComponent<BoxCollider>().size.x/2;
        float chosenRoomSize = chosenRoomObject.GetComponent<BoxCollider>().size.z/2 + chosenRoomObject.GetComponent<BoxCollider>().size.x/2;
        float roomOffset = chosenRoomSize + previousRoomSize + hallwayOffset;
        

    
        Vector3 directionToMove = startDoorway.forward;
        Vector3 amountToMove = directionToMove*roomOffset + previousRoomPostion;
        

        //place the room

        GameObject generatedRoom = Instantiate(chosenRoomObject, 
        new Vector3(amountToMove.x, chosenRoomObject.transform.position.y, amountToMove.z),
        Quaternion.Euler(new Vector3(0,rotationOffset,0)));


        //CHECK OVERLAP WITH OTHER ROOM
        //IF OVERLAP,REMOVE DOOR FROM LIST, DELETE ROOM,WALL OFF
        bool roomColliding = false;
        foreach(GameObject room in rooms){
           
             if(generatedRoom.GetComponent<BoxCollider>().bounds.Intersects(room.GetComponent<BoxCollider>().bounds)){
                roomColliding = true;

                
            }
        }

         foreach(GameObject room in roomsToAddOnLoopEnd){
           
             if(generatedRoom.GetComponent<BoxCollider>().bounds.Intersects(room.GetComponent<BoxCollider>().bounds)){
                roomColliding = true;

                
            }
        }
       
        if(roomColliding){
            DestroyImmediate(generatedRoom.gameObject);
            previousRoom.doorways.Remove(startDoorway);
            //wall of door
            Instantiate(wall,startDoorway.position,quaternion.identity).transform.parent = level.transform;
            
        }else{
       




        

        //doorways to list;
        Transform generatedDoorway = generatedRoom.GetComponent<Room>().doorways[chosenDoorwayIndex];
        doorways.Add(startDoorway);
        doorways.Add(generatedDoorway);
        //remove doorways from their rooms

        generatedRoom.GetComponent<Room>().doorways.Remove(generatedDoorway);
        previousRoom.doorways.Remove(startDoorway);
        roomsToAddOnLoopEnd.Add(generatedRoom);
        generatedLoops += 1;
        }

        }}
        rooms.AddRange(roomsToAddOnLoopEnd);

    
}


}


    
   
   
    

    


