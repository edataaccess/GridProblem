using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Philip Brown, edataaccess@hotmail.com,  770-310-2485,  03/28/2018
namespace ConsoleApplication1
{ 
    class Program
    {
        //entry point
        static void Main(string[] args)
        {
            GridProblem gp = new GridProblem();

            //solve problem            
            string outStr = gp.getListSubregionCOM(GridProblem.grid, 200);
            Console.OpenStandardOutput();
            Console.WriteLine("Center of Mass point by Subregions found.");
            Console.WriteLine("");
            Console.WriteLine(outStr);
            Console.WriteLine("");
            Console.WriteLine("press Enter to continue");
            Console.ReadLine();
        }
    }

    public class GridProblem
    {
        public static int[,] grid = new int[6, 6] {
            {  0,  80,  45,  95, 170, 145},  /* row for x=0 */
            {115, 210,  60,   5, 230, 220},  /* row for x=1 */
            {  5,   0, 145, 250, 245, 140},  /* row for x=2 */
            { 15,   5, 175, 250, 185, 160},  /* row for x=3 */
            {  0,   5,  95, 115, 165, 250},  /* row for x=4 */
            {  5,   0,  25,   5, 145, 250},  /* row for x=5 */
        };
        static int gridWidth = 6;
        static int gridHeight = 6;

        //constructor
        public GridProblem()
        {
        }

        //function to analyze input grid and return a list of Subregions' Center of Mass points.
        public string getListSubregionCOM( int[,] gridInput, int threshold)
        {
            //create empty subregion list.
            List<subregion> subregionList = new List<subregion>();

            int idx, jdx;
            //loop on x
            for (idx = 0; idx < gridWidth; idx++)
            {
                //loop on y
                for (jdx = 0; jdx < gridHeight; jdx++)
                {
                    // is current cell over threshold?
                    if (gridInput[idx, jdx] > threshold)
                    {
                        //create GridCell, since we will be storing this one.
                        GridCell newGC = new GridCell(idx, jdx, gridInput[idx,jdx]);
                        //Add current cell to a subregion...
                        {
                            int kdx;
                            bool foundAHome = false;
                            //find subregion it belongs in...
                            for (kdx = 0; kdx < subregionList.Count; kdx++)
                            {
                                subregion sr = subregionList[kdx];
                                //the current cell will be adjacent to any cell in an existing subregion
                                if (sr.isCellAdjacent(newGC.x, newGC.y))
                                {
                                    // current cell is adjacent to current subregion list.
                                    sr.Add(newGC);
                                    foundAHome = true;
                                }
                            }
                            //if not adjacent to any subregion, then create new subregion...
                            if (!foundAHome)
                            {
                                //create a new subregion
                                subregion newSR = new subregion();
                                subregionList.Add(newSR);

                                //add current cell to new subregion
                                newSR.Add(newGC);
                            }

                        } //add current cell to subregion

                    }//if over threshold

                }//end loop y

            }//end loop x

            //format output
            //return a list of Center Of Mass coordinates.
            StringBuilder sb = new StringBuilder();
            int mdx;
            for (mdx = 0; mdx < subregionList.Count; mdx++)
            {
                subregion sr = subregionList[mdx];
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(sr.centerMass.ToString());
            }

            //return output string.
            return sb.ToString();

        }//getListSubregionCOM

    }//GridProblem


    public class subregion :List<GridCell>
    {
        //List of GridCell objects

        //constructor
        public subregion()
        {
        }

        public bool isCellAdjacent(int cellX, int cellY)
        {
            if (this.Count < 1)
            {
                return false;
            }

            int idx;
            //loop thru cells in subregion
            for (idx = 0; idx < this.Count; idx++)
            {
                GridCell currCell = (GridCell)this[idx];
                if ( currCell.isCellAdjacent( cellX, cellY ) )
                {
                    return true;
                }
            }
            return false;
        }

        private decimal _centerMassX = 0;
        private decimal _centerMassY = 0;
        public CenterOfMass centerMass
        {
            get
            {
                //definition: center of mass for an empty subregion is 0,0.
                calcCenterMassX();
                calcCenterMassY();
                return new CenterOfMass(_centerMassX, _centerMassY);
            }
        }

        private void calcCenterMassX()
        {
            // calc _centerMassX
            int idx;
            GridCell g;
            decimal sumPositionalWeight = 0;
            decimal sumWeight = 0;
            for (idx = 0; idx < this.Count; idx++)
            {
                g = (GridCell)this[idx];
                sumPositionalWeight += g.x * g.weight;
                sumWeight += g.weight;
            }
            _centerMassX = sumPositionalWeight / sumWeight;
        }

        private void calcCenterMassY()
        {
            // calc _centerMassY
            int idx;
            GridCell g;
            decimal sumPositionalWeight = 0;
            decimal sumWeight = 0;
            for (idx = 0; idx < this.Count; idx++)
            {
                g = (GridCell)this[idx];
                sumPositionalWeight += g.y * g.weight;
                sumWeight += g.weight;
            }
            _centerMassY = sumPositionalWeight / sumWeight;
        }
    
    }//subregion


    public class GridCell
    {
        //constructor
        public GridCell(int x, int y, int weight)
        {
            _x = x;
            _y = y;
            _w = weight;
        }

        public bool isCellAdjacent(int newCellX, int newCellY)
        {
            if (newCellX >= (this.x - 1) &&
                newCellX <= (this.x + 1) &&
                newCellY >= (this.y - 1) &&
                newCellY <= (this.y + 1))
            {
                return true;
            }
            return false;
        }

        private int _x;
        public int x {
            get { return _x; }

            set { _x = value; }
        }

        private int _y;
        public int y {
            get { return _y; }
            set { _y = value; }
        }

        private int _w;
        public int weight
        {
            get { return _w; }
            set { _w = value; }
        }

    }//GridCell


    public class CenterOfMass
    {
        //constructor
        public CenterOfMass(decimal x, decimal y)
        {
            _x = x;
            _y = y;
        }

        private decimal _x;
        public decimal x
        {
            get { return _x; }
        }

        private decimal _y;
        public decimal y
        {
            get { return _y; }
        }

        public override string ToString(){
            
            return "(" + _x.ToString("#.##") + "," + _y.ToString("#.##") + ")";
        }

    }//CenterOfMass

}
