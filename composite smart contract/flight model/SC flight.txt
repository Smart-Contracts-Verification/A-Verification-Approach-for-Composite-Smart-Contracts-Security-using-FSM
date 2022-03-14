68// SPDX-License-Identifier: GPL-3.0

pragma solidity >=0.5.0 <=0.8.0;
pragma experimental ABIEncoderV2;
   
contract TravelAgency {
  
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

modifier onlyUser() {
for(uint i=0; i<Flight_counter; i++){
  require(msg.sender==Flights[i].Flight_CL, "Not user")
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
