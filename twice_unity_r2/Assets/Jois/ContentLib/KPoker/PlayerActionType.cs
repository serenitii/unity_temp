// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace KPoker
{

public enum PlayerActionType : byte
{
 Unknown = 0,
 Die = 1,
 Check = 2,
 Call = 3,
 Pping = 4,
 Double = 5,
 Quater = 6,
 Half = 7,
};

[System.Flags]
public enum PlayerBetType
{
 Normal = 0,
 AllIn = 1 << 0,
 SideBetting = 1 << 1, 
 MaxBet = 1 << 2
}

}