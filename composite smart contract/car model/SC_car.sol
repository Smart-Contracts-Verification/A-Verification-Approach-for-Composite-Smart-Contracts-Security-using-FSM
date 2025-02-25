68// SPDX-License-Identifier: GPL-3.0

pragma solidity >=0.5.0 <=0.8.0;
pragma experimental ABIEncoderV2;
   
contract Car  {
    
    
    
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
            cars[i].CL= msg.sender;

       }
    
    
}}
    
    
        
funtion () external payable {}
}
