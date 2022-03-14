# Verification-Approach-for-Solidity-based-Smart-Contracts-using-FSM
![tool architecture1](https://user-images.githubusercontent.com/79995136/150977983-4125557b-ead5-418e-940a-01ea44a59607.png)

In this work, we propose a novel approach to verify the security and the correctness of the composite smart contracts written in solidity in Ethereum blockchain. This approach is based on the finite state machine (FSM) to model the composite smart contracts behaviors. We consider up to seven security properties to be checked in the composite smart contract. We use two types of security verification. In the first type, we look for several kinds of vulnerabilities that are classified according to the relationships with the security properties in order to verify if a security property is not satisfied. To do this, we extract the FSM from the vulnerabilities logic and we translate them into Computation Tree Logic (CTL) formulae. These formulae will become standard properties predefined in our approach. In the second type and in order to increase the efficiency and complete the first type of verification, we provide specific properties to be verified. Such specific properties  consist of user-defined security properties based on smart contract context and written in CTL. Finally, we use the nuXmv symbolic model checker to verify the model against all properties. This approach is validated using a different set of solidity smart contracts.
