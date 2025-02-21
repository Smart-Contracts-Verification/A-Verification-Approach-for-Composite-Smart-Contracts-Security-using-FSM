# __Project Name__:  Composite Smart Contracts Verification
![Proposed Architecture](https://user-images.githubusercontent.com/79995136/177814402-9137507e-4589-44b2-8e6c-97ea9ea7cdaf.png)

## Summary

In this work, we propose a novel approach to verify the security and the correctness of composite smart contracts written in Solidity on the Ethereum blockchain. This approach is based on the finite state machine (FSM) to model the behaviors of composite smart contracts. We consider up to seven security properties to be checked in the composite smart contract.
We utilize two types of security verification:

1. **Vulnerability Analysis**: In the first type, we look for several kinds of vulnerabilities that are classified according to their relationships with security properties. This process verifies if a security property is not satisfied. To achieve this, we extract the FSM from the vulnerabilities' logic and translate them into Computation Tree Logic (CTL) formulae. These formulae become standard properties predefined in our approach.

2. **User-Defined Properties**: In the second type, and to enhance the efficiency of the first type of verification, we provide specific properties to be verified. These properties consist of user-defined security properties based on the context of the smart contract, written in CTL.

3. **Verification Stage**: Finally, we use the **nuXmv symbolic model checker** to verify the model against all properties. This approach is validated using a different set of Solidity smart contracts.




### Prerequisites

- Unix-based operating system (e.g. Debian, Arch, Mac OS)
- Ethereum testnet/ Ganache or Main RPC - You can get one in any of the providers of this list
    - [Chainstack](https://chainstack.com/)
    - [Ankr](https://ankr.com/)
    - [QuickNode](https://quicknode.com/)
    - [Alchemy](https://www.alchemy.com/)
    - [Infura](https://www.infura.io/)
- [BIP toolset](https://www-verimag.imag.fr/-BIP-Tools-93-?lang=en)
- nuXmv Symbolic Model Checker V2.x.x [nuXmv](https://nuxmv.fbk.eu/)

  ### Repository Structure
1. **SOL-To-Bip-UI-master**: 
   - This directory contains the source code for the tool itself. It includes all functionalities necessary for converting Solidity smart contracts into **BIP** (Behavioral Interaction Process) language and other verification components.

__**Note**:__ You can try the SOL-To-BIP with our user-friendly interface on the website version: [www.verifysolidity.com](http://www.verifysolidity.com).

2. **Composite Smart Contract**: 
   - This folder includes examples of composite smart contracts. Each example features:
     - The Solidity source code.
     - Transformation models into **BIP** and **SMV** (Symbolic Model Verification).
     - Verification results that demonstrate how these contracts adhere to specified properties.
     - **CTL** (Computation Tree Logic) formula properties that are used for verification.

3. **One Smart Contract**: 
   - Similar to the composite smart contracts, this directory provides examples specifically for single smart contracts. Each example is accompanied by:
     - The Solidity source code.
     - Transformation models into BIP and SMV.
     - Verification results and CTL formula properties.
  
# __Version__

Version: V1.0.0
