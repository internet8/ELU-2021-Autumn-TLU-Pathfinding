using System;

public class FloorConnection
{
    public bool stairs = false;
    public bool elevator = false;
    public bool ramp = false;
    public int[] connectionArray = new int[6]; // contains graph vertices

    public FloorConnection (bool stairs, bool elevator, bool ramp, int[] connectionArray) {
        this.elevator = elevator;
        this.stairs = stairs;
        this.ramp = ramp;
        this.connectionArray = connectionArray;
    }
}
