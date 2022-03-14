68// SPDX-License-Identifier: GPL-3.0

pragma solidity >=0.5.0 <=0.8.0;
pragma experimental ABIEncoderV2;


   
contract SCflight {
  
  //define the flight attributs
  struct  Flight{
      
      uint256 Flight_NB;
      string Flight_name;
      uint256 Flight_Price;
      string Flight_status;
      string Flight_type;
      address CL;
      
  }
  
    mapping (uint => Flight)  public Flights; 
       uint256  Flight_counter;
       address owner


    // define some data in the contractor
  constructor ()  public {
      
     createFlights(1, "A210",100 ,"Dispo", "Business",0x0000000000000000000000000000000000000000);
       createFlights(2, "A211",25 ,"Dispo", "Economy",0x0000000000000000000000000000000000000000);
        createFlights(3, "A10",605 ,"Dispo", "Business",0x0000000000000000000000000000000000000000);
	owner = msg.sender;
    }
    
modifier onlyAdmin() {
require (msg.sender == owner, "Not admin");
        _;
     }
    
// create a flight with a certain attribute
    function createFlights(uint256 F_nb, string memory F_name, uint256 F_price, string memory F_status,string memory F_type, address F_client) public onlyAdmin {
        
        Flights[Flight_counter]= Flight(F_nb, F_name, F_price,F_status, F_type,F_client);
        Flight_counter ++;
    } 

// search the minimal flights price and return it;
    function Search_flight_PRICE(uint256 Total_Price) external   view  returns(uint256){
        require(Total_Price >0,"Price should be greater than 0");
        
       uint Pminflights= Flights[0].Flight_Price;
      for(uint i=0; i <= Flight_counter; i++){
         
         if(Flights[i].Flight_Price < Pminflights && keccak256(abi.encodePacked((Flights[i].Flight_status))) == keccak256(abi.encodePacked(("Dispo")))){
              Pminflights = Flights[i].Flight_Price ;
             // idd = Flights[i].Flight_NB;
             }
      }

        if(Total_Price> Pminflights){    
            
         return(Pminflights);     
         
        }else   { 
          //  revert(" No flights for the price you entered, please increase it ") ;
            return (Pminflights);
            }        
}
    
// get infos about a specific flight according to the ID
function getFlight_by_ID(uint id, address _adr) public view returns(Flight memory){

   for(uint i=0;i< Flight_counter;i++){
       if(Flights[i].Flight_NB == id && Flights[i].Flight_CL == _adr){
            return Flights[id-1];
           
       }
   }
}

function Book_FLIGHT (uint256  _id) external {
   
   for(uint i=0;i< Flight_counter;i++){
      if(Flights[i].Flight_NB == _id  && Flights[i].Flight_CL ==0x0000000000000000000000000000000000000000 && keccak256(abi.encodePacked((Flights[i].Flight_status))) == keccak256(abi.encodePacked(("Dispo"))))  {
            Flights[i].Flight_status= "reserved";
            Flights[i].CL=msg.sender;}}

  
function kill() public {
        selfdestruct(owner);}

funtion () external payable {}
}

      

contract SChotel  {
    
    
    struct  Hotel{
      
      uint256 Hotel_nb;
      string Hotel_name;
      uint256 Hotel_Price;
      string Hotel_status;
      string Hotel_type;
      address CL;
  }
    mapping (uint=>  Hotel) public  Hotels; 
    
  constructor ()  public{
      createHotels(1, "Lagona Hotel",100 , "Dispo","Suite room",0x0000000000000000000000000000000000000000);
       createHotels(2, "Rotana Hotel",75 ,"Dispo", "suite room",0x0000000000000000000000000000000000000000);
        createHotels(3, "Moulin vert Hotel",30 ,"rented" ,"Room",0x0000000000000000000000000000000000000000);
        createHotels(4, " Hilton Hotel",19 ,"Dispo", "Room",0x0000000000000000000000000000000000000000);
    } 
   uint256 Hotel_counter =0;

    function createHotels(uint256 H_nb, string memory H_name, uint256 H_price,string memory H_dispo ,string memory H_type, address H_client) public{
        
       
        Hotels[Hotel_counter]= Hotel(H_nb, H_name, H_price,H_dispo, H_type,H_client);
        Hotel_counter ++;
    }
    
 function Search_Hotel(uint256 Total_Price1) external  view  returns(  uint256 Price_min_hotel  ){
               
       require(Total_Price1 >0,"Price should be greater than 0");

       uint Pminhotel=Hotels[0].Hotel_Price;
      
      for(uint i=0; i<Hotel_counter; i++){
         
         if(Hotels[i].Hotel_Price < Pminhotel && keccak256(abi.encodePacked((Hotels[i].Hotel_status))) == keccak256(abi.encodePacked(("Dispo")))){
              Pminhotel =Hotels[i].Hotel_Price;
             }
         
      }
        
        if(Total_Price1 > Pminhotel){    
          
        return(Pminhotel);     
}


function getHotel_by_ID(uint _id) public view returns(Hotel memory){
   for(uint i=0;i< Hotel_counter;i++){
       if(Hotels[i].Hotel_nb == _id){
            return Hotels[_id-1];
           
       }
   }
}

function Book_HOTEL (uint256  _id) external   {
   
   for(uint i=0;i< Hotel_counter;i++){
       if(Hotels[i].Hotel_nb == _id && keccak256(abi.encodePacked((Hotels[i].Hotel_status))) == keccak256(abi.encodePacked(("Dispo")))){
            Hotels[i].Hotel_status= "reserved";
            Hotels[i].CL=msg.sender;
       } 
    
}}
    
funtion () external payable {}
}


contract SCcar  {
    
    
    
    struct  car{
      
      uint256 car_nb;
      string car_name;
      uint256 car_Rent_Price;
      string car_status;
      string car_type;
      address CL;
  }
    mapping (uint=>  car)  public cars; 
    
  constructor () public {
      createcars(1, "Toyota yaris",10 ,"Dispo", "classic",0x0000000000000000000000000000000000000000);
       createcars(2, "yukon",75 , "Dispo","4*4",0x0000000000000000000000000000000000000000);
        createcars(3, "bugatti",300 ,"Dispo", "super car",0x0000000000000000000000000000000000000000);
        createcars(4, " ferrari",250 , "Dispo","super car",0x0000000000000000000000000000000000000000);
        createcars(5, " Renault",8 , "Dispo","classic",0x0000000000000000000000000000000000000000);
        createcars(6, "G36 class ",190 , "Dispo","4*4",0x0000000000000000000000000000000000000000);
    } 
   uint256 car_counter =0;

    function createcars(uint256 C_nb, string memory C_name, uint256 C_price, string memory C_status, string memory C_type, address C_client) private{
        
       
        cars[car_counter]= car(C_nb, C_name, C_price,C_status, C_type,C_client);
         //cars[car_counter].push((car(C_nb, C_name, C_price,C_status, C_type)));
        add.(car_counter,1);
    }

    function Search_Car(uint256 Total_Price2) external  view  returns( uint256 Price_min_car){
       require(Total_Price2 >0,"Price should be greater than 0");
       uint256 Pmincar= cars[0].car_Rent_Price;
    
      for(uint i=0; i<car_counter; i++){
         
         if(cars[i].car_Rent_Price < Pmincar){
              Pmincar = cars[i].car_Rent_Price;
             }
      }
        
        if(Total_Price2 > Pmincar){    
            
        return(Pmincar);     
         
        }else {return (Pmincar);
           // revert("No cars for the price you entered, please increase it"); 
            
        }
}
     
    

function getCar_by_ID(uint _id) private view returns(car memory ){
  require(_id>0,"ID incorrect");
   
   for(uint i=0;i< car_counter;i++){
       if(cars[i].car_nb == _id){
            return cars[_id-1];
           
       }
   }
}

function Book_CAR (uint256  _id) external   {
    
    
    require(_id>0,"ID incorrect");
   
   for(uint i=0;i< car_counter;i++){
       if(cars[i].car_nb == _id && keccak256(abi.encodePacked((cars[i].car_status))) == keccak256(abi.encodePacked(("Dispo")))){
            cars[i].car_status= "reserved";
            cars[i].CL= msg.sender;}}

       }
        
funtion () external payable {}
}
 
    contract Book {
      SChotel HO = new SChotel(); 
        SCcar CA =  new SCcar();
     SCflight TA= new SCflight();
   
      
         //return infos about a specific flight

     
               function Book_flight(uint256 _id)   public  {
               
              TA.send(2);
              TA.Book_FLIGHT(_id); }    
       
          function Book_car(uint256 _id)   public  {
          
            
            	CA.send(2);
           	 CA.Book_CAR(_id);
        
           // cars[_id-1].car_status="reserved"; }    
        
        
       function Book_hotel(uint256 _id)  public  {
           
            
       		 HO.send(2);
          	HO.Book_HOTEL(_id);} } 
            
  
