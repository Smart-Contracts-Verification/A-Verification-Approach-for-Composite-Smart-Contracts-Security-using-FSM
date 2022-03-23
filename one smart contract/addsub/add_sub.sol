contract SimpleDAO {

function sub(uint256 a, uint256 b) internal pure returns (uint256) {
    assert(b <= a); // Preventative Technique
    return a - b;
  }

  function add(uint256 a, uint256 b) internal pure returns (uint256) {
	uint c = a + b;
	assert(c>=a);
	return c;
	    // Preventative Technique
   
    
  }
}

}
