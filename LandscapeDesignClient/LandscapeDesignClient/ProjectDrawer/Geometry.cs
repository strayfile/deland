using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LandscapeDesignClient.ProjectDrawer
{
    static class GeometryFunctions
    {
        private static int WhichSide(Point PointToBeEvaluated, Point StartPointOnLine, Point EndPointOnLine)
        {
            double ReturnvalueEquation;
            ReturnvalueEquation = ((PointToBeEvaluated.Y - StartPointOnLine.Y)
                                   * (EndPointOnLine.X - StartPointOnLine.X)) - ((EndPointOnLine.Y - StartPointOnLine.Y)
                                   * (PointToBeEvaluated.X - StartPointOnLine.X));

            if (ReturnvalueEquation > 0)
                return -1;
            else if (ReturnvalueEquation == 0)
                return 0;
            else
                return 1;
        }

        private static double DistanceFromLine(Point LinePoint1, Point LinePoint2, Point TestPoint)
        {
            double d;
            d = Math.Abs((LinePoint2.X - LinePoint1.X) * (LinePoint1.Y - TestPoint.Y) - (LinePoint1.X - TestPoint.X) * (LinePoint2.Y - LinePoint1.Y));
            d = d / Math.Sqrt(Math.Pow((LinePoint2.X - LinePoint1.X), 2) + Math.Pow((LinePoint2.Y - LinePoint1.Y), 2));
            return d;
        }

        private static double DistanceBetweenPoints(Point Point1, Point Point2)
        {
            return Math.Sqrt(Math.Pow((Point1.X - Point2.X), 2) + Math.Pow((Point1.Y - Point2.Y), 2));
        }

        private static double Angles(Point Point1, Point Point2, Point Point3)
        {
            double result;
            double a, b, c;
            c = DistanceBetweenPoints(Point1, Point3);
            b = DistanceBetweenPoints(Point1, Point2);
            a = DistanceBetweenPoints(Point2, Point3);
            result = Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * b * a));
            return result;
        }
        private static Point Normal2D(Point Point1, Point point2, double theta = Math.PI / 2)
        {
            Point p = new Point
            {
                X = Math.Cos(theta) * (point2.X - Point1.X) - Math.Sin(theta) * (point2.Y - Point1.Y) + Point1.X,
                Y = Math.Sin(theta) * (point2.X - Point1.X) + Math.Cos(theta) * (point2.Y - Point1.Y) + Point1.Y
            };
            return p;
        }

        public static PointCollection InsertPoint(PointCollection OriginalPointColletion, Point NewPoint, bool IsPolygon = false)
        {
            PointCollection result = new PointCollection();
            result = OriginalPointColletion.Clone();
            double min_distance = double.MaxValue;

            double temp_distance;
            int index = 0;

            if (IsPolygon)
            {
                List<VectorLine> VectorLinesCalc = new List<VectorLine>();
                VectorLinesCalc = CalculateAllAnglesPolygon(OriginalPointColletion);

                for (int i = 0; i <= OriginalPointColletion.Count - 1; i++)
                {
                    if (i == OriginalPointColletion.Count - 1)
                        temp_distance = DistanceFromLine2(NewPoint, VectorLinesCalc[i], VectorLinesCalc[0]);
                    else
                        temp_distance = DistanceFromLine2(NewPoint, VectorLinesCalc[i], VectorLinesCalc[i + 1]);
                    if (temp_distance < min_distance)
                    {
                        min_distance = temp_distance;
                        if (i != OriginalPointColletion.Count - 1)
                            index = i + 1;
                        else
                            index = 0;
                    }
                }
                if (index != 0)
                    result.Insert(index, NewPoint);
                else
                    result.Add(NewPoint);
            }
            else
            {
                List<VectorLine> VectorLinesCalc = new List<VectorLine>();
                VectorLinesCalc = CalculateAllAngles(OriginalPointColletion);

                for (int i = 0; i <= OriginalPointColletion.Count - 2; i++)
                {
                    temp_distance = DistanceFromLine2(NewPoint, VectorLinesCalc[i], VectorLinesCalc[i + 1]);
                    if (temp_distance < min_distance)
                    {
                        min_distance = temp_distance;
                        index = i + 1;
                    }
                }

                if (DistanceBetweenPoints(OriginalPointColletion[0], NewPoint) < min_distance)
                {
                    min_distance = DistanceBetweenPoints(OriginalPointColletion[0], NewPoint);
                    index = 0;
                }

                if (DistanceBetweenPoints(OriginalPointColletion[OriginalPointColletion.Count - 1], NewPoint) < min_distance)
                {
                    index = -1;
                    min_distance = DistanceBetweenPoints(OriginalPointColletion[OriginalPointColletion.Count - 1], NewPoint);
                }

                if (index != -1)
                    result.Insert(index, NewPoint);
                else
                    result.Add(NewPoint);
            }
            return result;
        }

        private static double DistanceFromLine2(Point TestPoint, VectorLine vector1, VectorLine vector2)
        {
            int FirstTest, SecondTest;

            FirstTest = WhichSide(TestPoint, vector1.Point1, vector1.Point2);
            SecondTest = WhichSide(TestPoint, vector2.Point1, vector2.Point2);

            if (FirstTest != SecondTest | (FirstTest == 0 & SecondTest == 0))
            {
                double dist;

                dist = DistanceFromLine(vector1.Point1, vector2.Point1, TestPoint);

                double FromFirstPoint, FromSecondPoint;
                FromFirstPoint = DistanceBetweenPoints(TestPoint, vector1.Point1);
                FromSecondPoint = DistanceBetweenPoints(TestPoint, vector2.Point1);
                double angl;
                if (FromFirstPoint <= FromSecondPoint)
                {
                    angl = Angles(vector2.Point1, vector1.Point1, TestPoint);
                    if (angl > Math.PI / 2)
                        return FromFirstPoint;
                }
                else
                {
                    angl = Angles(vector1.Point1, vector2.Point1, TestPoint);

                    if (angl > Math.PI / 2)
                        return FromSecondPoint;
                }

                return dist;
            }
            else
                return double.MaxValue;
        }

        public static List<VectorLine> CalculateAllAnglesPolygon(PointCollection OriginalPointCollection)
        {
            List<VectorLine> result = new List<VectorLine>();
            for (int i = 0; i <= OriginalPointCollection.Count - 1; i++)
            {
                VectorLine NewVectorLine = new VectorLine();
                if (i == 0)
                {
                    NewVectorLine.Point1 = OriginalPointCollection[i];
                    double angl = Angles(OriginalPointCollection[OriginalPointCollection.Count - 1], OriginalPointCollection[i], OriginalPointCollection[i + 1]);

                    NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[i + 1], angl / 2);

                    result.Add(NewVectorLine);
                }
                else if (i == OriginalPointCollection.Count - 1)
                {
                    NewVectorLine.Point1 = OriginalPointCollection[i];
                    double angl = Angles(OriginalPointCollection[i - 1], OriginalPointCollection[i], OriginalPointCollection[0]);

                    int PreviousAngle, NextAngle;
                    PreviousAngle = WhichSide(result[i - 1].Point2, OriginalPointCollection[i - 1], OriginalPointCollection[i]);
                    NextAngle = WhichSide(OriginalPointCollection[i], OriginalPointCollection[i - 1], OriginalPointCollection[0]);
                    if (PreviousAngle == NextAngle)
                        NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[0], angl / 2);
                    else
                        NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[0], (2 * Math.PI - angl) / 2);
                    result.Add(NewVectorLine);
                }
                else
                {
                    NewVectorLine.Point1 = OriginalPointCollection[i];
                    double angl = Angles(OriginalPointCollection[i - 1], OriginalPointCollection[i], OriginalPointCollection[i + 1]);

                    int PreviousAngle, NextAngle;
                    PreviousAngle = WhichSide(result[i - 1].Point2, OriginalPointCollection[i - 1], OriginalPointCollection[i]);
                    NextAngle = WhichSide(OriginalPointCollection[i + 1], OriginalPointCollection[i - 1], OriginalPointCollection[i]);
                    if (PreviousAngle == NextAngle)
                        NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[i + 1], angl / 2);
                    else
                        NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[i + 1], (2 * Math.PI - angl) / 2);
                    result.Add(NewVectorLine);
                }
            }

            return result;
        }

        public static List<VectorLine> CalculateAllAngles(PointCollection OriginalPointCollection)
        {
            List<VectorLine> result = new List<VectorLine>();
            for (int i = 0; i <= OriginalPointCollection.Count - 1; i++)
            {
                VectorLine NewVectorLine = new VectorLine();
                if (i == 0)
                {
                    NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[i + 1]);
                    NewVectorLine.Point1 = OriginalPointCollection[i];
                    result.Add(NewVectorLine);
                }
                else if (i == OriginalPointCollection.Count - 1)
                {
                    NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[i - 1], 3 * Math.PI / 2);
                    NewVectorLine.Point1 = OriginalPointCollection[i];
                    result.Add(NewVectorLine);
                }
                else
                {
                    NewVectorLine.Point1 = OriginalPointCollection[i];
                    double angl = Angles(OriginalPointCollection[i - 1], OriginalPointCollection[i], OriginalPointCollection[i + 1]);

                    int PreviousAngle, NextAngle;
                    PreviousAngle = WhichSide(result[i - 1].Point2, OriginalPointCollection[i - 1], OriginalPointCollection[i]);
                    NextAngle = WhichSide(OriginalPointCollection[i + 1], OriginalPointCollection[i - 1], OriginalPointCollection[i]);
                    if (PreviousAngle == NextAngle)
                        NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[i + 1], angl / 2);
                    else
                        NewVectorLine.Point2 = Normal2D(OriginalPointCollection[i], OriginalPointCollection[i + 1], (2 * Math.PI - angl) / 2);
                    result.Add(NewVectorLine);
                }
            }
            return result;
        }
        
        public class VectorLine
        {
            public Point Point1 = new Point();
            public Point Point2 = new Point();
        }
    }
}