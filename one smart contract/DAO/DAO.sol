contract SimpleDAO {
   uint public credit;
    
  function donate(uint amount) {
    credit += amount;
  }
    
  function withdraw(uint amount) {
    if (credit[msg.sender]>= amount) {
      
      msg.sender.call.value(amount)();
      credit[msg.sender]-=amount;
    }
  }  

  function queryCredit() returns (uint){
    return credit;
  }
}
