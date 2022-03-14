68// SPDX-License-Identifier: GPL-3.0

pragma solidity >=0.5.0 <=0.8.0;
pragma experimental ABIEncoderV2;
   
contract Hotel1  {
    
    
    struct  Hotel{
      
      uint256 Hotel_nb;
      string Hotel_name;
      uint256 Hotel_Price;
      string Hotel_status;
      string Hotel_type;
      address CL;
  }
    mapping (uint=>  Hotel) public  Hotels; 
    address owner
  constructor ()  public{
      createHotels(1, "Lagona Hotel",100 , "Dispo","Suite room",0x0000000000000000000000000000000000000000);
       createHotels(2, "Rotana Hotel",75 ,"Dispo", "suite room",0x0000000000000000000000000000000000000000);
        createHotels(3, "Moulin vert Hotel",30 ,"rented" ,"Room",0x0000000000000000000000000000000000000000);
        createHotels(4, " Hilton Hotel",19 ,"Dispo", "Room",0x0000000000000000000000000000000000000000);
    } 
   uint256 Hotel_counter =0;


modifier onlyAdmin() {
require (msg.sender == owner, "Not admin");
        _;
     }

modifier onlyUser() {
for(uint i=0; i<Hotel_counter; i++){
  require(msg.sender==Hotels[i].Hotel_CL)
        _;
     }

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


function getHotel_by_ID(uint _id) public view returns(Hotel memory) onlyUser{
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
