﻿using System;
using System.IO;
using System.Collections.Generic;



class Matrix
{
    #region Class Fields
    private double[,] _data;

    /// <summary>
    /// Number of rows in this matrix
    /// </summary>
    /// <value>number of rows</value>
    public int Row
    {
        get { return _data.GetLength(0); }
    }

    /// <summary>
    /// Number of columns in this matrix
    /// </summary>
    /// <value>number of columns</value>
    public int Column
    {
        get { return _data.GetLength(1); }
    }

    /// <summary>
    /// The transpose of the matrix
    /// </summary>
    /// <value>the transposed matrix</value>
    public Matrix T
    {
        // A = Row X Column
        // A_T = Column X Row 
        get
        {
            Matrix TransposedMatrix = new Matrix(this.Column, this.Row);
            for (int row = 0; row < this.Row; row++)
            {
                for (int col = 0; col < this.Column; col++)
                {
                    // change the position of row index and col index
                    TransposedMatrix[col, row] = this[row, col];
                }
            }
            return TransposedMatrix;
        }
    }

    /// <summary>
    /// return the shape of the matrix as a 1D int array[row,col] (for creating new matrix) 
    /// </summary>
    /// <value></value>
    public int[] Shape
    {
        get
        {
            int[] shape = { Row, Column };
            return shape;
        }
    }

    /// <summary>
    /// Return the size of the matrix as a string (a space between operator and operand). e.g. "5 X 3"
    /// </summary>
    /// <value></value>
    public string Size
    {
        get { return $"{this.Row} X {this.Column}"; }
    }

    //[] overload
    /// <summary>
    /// Gets the element of the matrix using the given row and col index
    /// </summary>
    /// <value></value>
    public double this[int row, int col]
    {

        get
        {
            return _data[row, col];
        }
        set
        {
            _data[row, col] = value;
        }
    }

    /// <summary>
    /// only works for 1 column matrix, get the specific value
    /// </summary>
    /// <value></value>
    public double this[int row]
    {
        get
        {
            if (this.Column == 1)
            {
                return _data[row, 0];
            }
            else
            {
                throw new ArgumentException("Invalid indexing");
            }
        }
        set
        {
            if (this.Column == 1)
            {

                _data[row, 0] = value;
            }
            else
            {
                throw new ArgumentException("Invalid indexing");
            }
        }
    }



    #endregion

    #region Class Constructors
    /// <summary>
    /// Constructs an empty matrix with specific row and column
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public Matrix(int row, int col)
    {
        _data = new double[row, col];
    }

    /// <summary>
    /// Constructs a matrix using 2D double array
    /// </summary>
    /// <param name="input">a 2D double array</param>
    public Matrix(double[,] input)
    {
        _data = input;
    }

    /// <summary>
    /// Constructs a one row matrix using a 1D double array
    /// </summary>
    /// <param name="input">one dimension double array</param>
    public Matrix(double[] input)
    {
        int numCols = input.GetLength(0);

        Matrix matrix = new Matrix(1, numCols);
        for (int col = 0; col < numCols; col++)
        {
            matrix[1, col] = input[col];
        }
        _data = matrix.ToDoubleArray();
    }

    /// <summary>
    /// Constructs a matrix using a data file or text file
    /// </summary>
    /// <param name="filePath">the file containing a matrix</param>
    public Matrix(string filePath)
    {
        bool isDataFile = CheckFileExtension(filePath, "data");
        bool isTextFile = CheckFileExtension(filePath, "txt");
        // if input is a data file
        if (isDataFile)
        {
            // This uses the ArrayLoader class from ITD121 PST1
            _data = Load2DArray(filePath);
        }
        else if (isTextFile)// The delimiter is ,
        {
            char delimiter = ',';
            // Loads data from the text file
            string[] lines = File.ReadAllLines(filePath);
            string line = lines[0];

            // Initialises the _data
            int numRows = lines.Length;
            int numCols = line.Split(delimiter).Length;
            _data = new double[numRows, numCols];

            // Fills the data array
            string[] cols;
            try
            {
                for (int row = 0; row < numRows; row++)
                {
                    line = lines[row];
                    cols = line.Split(delimiter);
                    for (int col = 0; col < numCols; col++)
                    {
                        _data[row, col] = double.Parse(cols[col]);
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("The text file's row or column is inconsistent!");
            }
        }

    }

    /// <summary>
    /// Construct a matrix using an 1D int array, usually matrix.Shape
    /// </summary>
    /// <param name="shape">1D int array containing [row,col]</param>
    public Matrix(int[] shape)
    {
        if (shape.Length != 2) { throw new Exception("input must be a 1D int[] containing the shape"); }
        int row = shape[0];
        int col = shape[1];
        _data = new Matrix(row, col)._data;
    }

    /// <summary>
    /// Constructs a new matrix by copying the input matrix
    /// </summary>
    /// <param name="matrix">the matrix to be cloned</param>
    public Matrix(Matrix matrix)
    {
        _data = matrix.ToDoubleArray();
    }
    /// <summary>
    /// Creates a matrix with random numbers
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public static Matrix RandomMatrix(int row, int col)
    {
        Random rng = new Random();
        Matrix result = new Matrix(row, col);
        for (int rowIndex = 0; rowIndex < result.Row; rowIndex++)
        {
            for (int colIndex = 0; colIndex < result.Column; colIndex++)
            {
                double randomNum = rng.NextDouble() * 100;
                result[rowIndex, colIndex] = randomNum;
            }
        }
        return result;
    }



    #endregion

    #region Operator Helpers


    /// <summary>
    /// Matrix dot product
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix Dot(Matrix left, Matrix right)
    {
        Matrix result = left.Dot(right);
        return result;
    }

    /// <summary>
    /// Element-wise addition, broadcasting
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix Add(Matrix left, Matrix right)
    {
        Matrix[] ab = BroadCasting(left, right);
        Matrix a = ab[0];
        Matrix b = ab[1];
        Matrix result = new Matrix(a.Row, a.Column);

        // check the size
        if (a.Size != b.Size)
        {
            Console.WriteLine("Cannot add these two matries");
            throw new ArgumentException($"The size of left matrix: {a.Size} is not equal to the right matrix: {b.Size}");
        }
        for (int row = 0; row < result.Row; row++)
        {
            for (int col = 0; col < result.Column; col++)
            {
                result[row, col] = a[row, col] + b[row, col];
            }
        }

        return result;
    }

    /// <summary>
    /// Element-wise substraction, broadcasting
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix Substract(Matrix left, Matrix right)
    {
        Matrix[] ab = BroadCasting(left, right);
        Matrix a = ab[0];
        Matrix b = ab[1];
        Matrix result = new Matrix(a.Row, a.Column);

        // check the size
        if (a.Size != b.Size)
        {
            Console.WriteLine("Cannot substract these two matries");
            throw new ArgumentException($"The size of left matrix: {a.Size} is not equal to the right matrix: {b.Size}");
        }
        for (int row = 0; row < result.Row; row++)
        {
            for (int col = 0; col < result.Column; col++)
            {
                result[row, col] = a[row, col] - b[row, col];
            }
        }

        return result;
    }

    /// <summary>
    /// Element-wise multiplication, broadcasting
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right matrix</param>
    /// <returns>a matrix</returns>
    public static Matrix Multiply(Matrix left, Matrix right)
    {
        Matrix[] ab = BroadCasting(left, right);
        Matrix a = ab[0];
        Matrix b = ab[1];

        Matrix result = new Matrix(a.Shape);
        for (int row = 0; row < a.Row; row++)
        {
            for (int col = 0; col < a.Column; col++)
            {
                result[row, col] = a[row, col] * b[row, col];
            }
        }
        return result;
    }

    /// <summary>
    /// Element-wise division, broadcasting
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix Divide(Matrix left, Matrix right)
    {
        Matrix[] ab = BroadCasting(left, right);
        Matrix a = ab[0];
        Matrix b = ab[1];

        Matrix result = new Matrix(a.Shape);
        for (int row = 0; row < a.Row; row++)
        {
            for (int col = 0; col < a.Column; col++)
            {
                result[row, col] = a[row, col] / b[row, col];
            }
        }
        return result;
    }


    /// <summary>
    /// element-wise addition
    /// </summary>
    /// <param name="right"></param>
    /// <returns>new matrix</returns>
    public Matrix Add(Matrix right)
    {
        Matrix result = Add(this, right);
        return result;
    }

    /// <summary>
    /// element-wise substraction
    /// </summary>
    /// <param name="right"></param>
    /// <returns>new matrix</returns>
    public Matrix Substract(Matrix right)
    {
        Matrix result = Substract(this, right);

        return result;
    }


    /// <summary>
    /// element-wise multiplication, broadcasting
    /// </summary>
    /// <param name="num">the number to be multiplied</param>
    /// <returns>new matrix</returns>
    public Matrix Multiply(double num)
    {
        // Truns the number into a 1 X 1 matrix 
        Matrix numMatrix = new Matrix(1, 1).SetNum(num);

        Matrix result = Multiply(this, numMatrix);

        return result;
    }

    /// <summary>
    /// Matrix dot product
    /// </summary>
    /// <param name="right"></param>
    /// <returns></returns>
    public Matrix Dot(Matrix right)
    {
        Matrix output;

        // check whether row == col
        if (this.Column == right.Row)//valid
        {
            output = new Matrix(row: this.Row, col: right.Column);// output matrix initialize

            for (int row = 0; row < this.Row; row++)
            {
                for (int col = 0; col < right.Column; col++)
                {
                    double cell = 0;
                    for (int leftColumnIndex = 0; leftColumnIndex < this.Column; leftColumnIndex++)
                    {
                        cell = cell + this[row, leftColumnIndex] * right[leftColumnIndex, col];
                    }
                    output[row, col] = cell;
                }
            }
            return output;
        }
        else// if input is invalid
        {
            Console.WriteLine("left [column] and right [row] is not the same");
            Console.WriteLine($"{this.Column} != {right.Row}");
            throw new ArgumentException();
        }
    }


    // for overloadding == and !=
    public bool Equals(Matrix other)
    {
        return this == other;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        var other = (Matrix)obj;
        return this == other;
    }

    #endregion

    #region Operator Overloads

    /// <summary>
    /// Matrix dot product
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator *(Matrix left, Matrix right)
    {
        return Dot(left,right);
    }

    /// <summary>
    /// Broadcasting the two input matries
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>a matrix 1D array after broadcasting</returns>
    private static Matrix[] BroadCasting(Matrix a, Matrix b)
    {
        if (a.Size == b.Size) // No need to perform broadcasting
        {
            return new Matrix[] { a, b };
        }

        Matrix left = null;
        Matrix right = null;
        // case 1: (numRows,numCols) (1,1)
        // if one of them is a 1 X 1 matrix
        if (a.Size == "1 X 1")
        {
            left = new Matrix(b.Shape).SetNum(a[0]);
            right = new Matrix(b);
        }
        else if (b.Size == "1 X 1")
        {
            left = new Matrix(a);
            right = new Matrix(a.Shape).SetNum(b[0]);
        }
        // case 2: 400 X 100 and 4 X 1
        // for two matries which have the same row and one of them has 1 column
        else if (a.Row == b.Row)
        {
            if (a.Column == 1) // Expands a = 4 X 1
            {
                Matrix rowAccumulator = new Matrix(1, b.Shape[1]);
                // One single row
                Matrix row;
                for (int i = 0; i < a.Row; i++)
                {
                    row = new Matrix(1, b.Shape[1]).SetNum(a[i]);
                    rowAccumulator = rowAccumulator.BottomConcatenate(row);
                }
                // Remove the first row
                rowAccumulator = rowAccumulator.RemoveRow(0);

                left = rowAccumulator;
                right = new Matrix(b);
            }
            else // Expands b = 4 X 1
            {
                Matrix rowAccumulator = new Matrix(1, a.Shape[1]);
                // One single row
                Matrix row;
                for (int i = 0; i < b.Row; i++)
                {
                    row = new Matrix(1, a.Shape[1]).SetNum(b[i]);
                    rowAccumulator = rowAccumulator.BottomConcatenate(row);
                }
                // Remove the first row
                rowAccumulator = rowAccumulator.RemoveRow(0);

                left = new Matrix(a);
                right = rowAccumulator;
            }
        }
        // case 3: 1 X 3 and 4 X 3
        // for tow matries which have the same col and one of them has 1 row
        else if (a.Column == b.Column)
        {
            Matrix rowAccumulator = new Matrix(1, a.Column);
            if (a.Row == 1) // Expands left matrix by copying row down
            {
                Matrix row = new Matrix(a);
                for (int i = 0; i < b.Row; i++)
                {
                    rowAccumulator = rowAccumulator.BottomConcatenate(row);
                }
                rowAccumulator = rowAccumulator.RemoveRow(0);

                left = rowAccumulator;
                right = new Matrix(b);
            }
            else // Expands the right matrix
            {
                Matrix row = new Matrix(b);
                for (int i = 0; i < a.Row; i++)
                {
                    rowAccumulator = rowAccumulator.BottomConcatenate(row);
                }
                rowAccumulator = rowAccumulator.RemoveRow(0);

                left = new Matrix(a);
                right = rowAccumulator;
            }
        }
        else // Throws an exception
        {
            throw new ArgumentException($"These two matries cannot be broadcated! a = {a.Size}, b = {b.Size}");
        }

        return new Matrix[] { left, right };
    }

    /// <summary>
    /// Element-wise addition, broadcasting
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator +(Matrix left, Matrix right)
    {
        Matrix[] ab = BroadCasting(left, right);

        Matrix a = ab[0];
        Matrix b = ab[1];

        Matrix result;
        result = a.Add(b);

        return result;
    }

    /// <summary>
    /// Element-wise addition, make a matrix full of the number then add the new matrix with the corresopnding matrix
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator +(Matrix left, double right)
    {
        Matrix rightMatrix = new Matrix(left.Shape).SetNum(right);
        Matrix result = left + rightMatrix;
        return result;
    }

    /// <summary>
    /// Element-wise addition, make a matrix full of the number then add the new matrix with the corresopnding matrix
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator +(double left, Matrix right)
    {
        // shape has to be the same as the right matrix
        Matrix leftMatrix = new Matrix(right.Shape).SetNum(left);
        Matrix result = leftMatrix + right;
        return result;
    }

    /// <summary>
    /// Element-wise substraction
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right number</param>
    /// <returns></returns>
    public static Matrix operator -(Matrix left, double right)
    {
        Matrix rightMatrix = new Matrix(left.Shape).SetNum(right);
        Matrix result = left - rightMatrix;
        return result;
    }

    /// <summary>
    /// Element-wise substraction
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator -(double left, Matrix right)
    {
        // shape has to be the same as the right matrix
        Matrix leftMatrix = new Matrix(right.Shape).SetNum(left);
        Matrix result = leftMatrix - right;
        return result;
    }

    /// <summary>
    /// Element-wise substraction
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator -(Matrix left, Matrix right)
    {
        
        Matrix result = Substract(left,right);

        return result;
    }

    /// <summary>
    /// Element-wise multiplication
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator *(Matrix left, double right)
    {
        // Truns the number into a 1 X 1 matrix
        Matrix rightMatrix = new Matrix(1, 1).SetNum(right);
        Matrix result = Multiply(left, rightMatrix);

        return result;
    }

    /// <summary>
    /// Element-wise multiplication
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator *(double left, Matrix right)
    {
        // Turns the number into a 1 X 1 matrix
        Matrix leftMatrix = new Matrix(1, 1).SetNum(left);
        Matrix result = Multiply(leftMatrix, right);

        return result;
    }

    /// <summary>
    /// element wise division
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator /(Matrix left, double right)
    {
        Matrix rightMatrix = new Matrix(left.Shape).SetNum(right);
        Matrix result = Divide(left, rightMatrix);

        return result;
    }

    /// <summary>
    /// Element wise division
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Matrix operator /(double left, Matrix right)
    {
        Matrix leftMatrix = new Matrix(right.Shape).SetNum(left);
        Matrix result = Divide(leftMatrix, right);

        return result;
    }

    /// <summary>
    /// element-wise division
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right matrix</param>
    /// <returns></returns>
    public static Matrix operator /(Matrix left, Matrix right)
    {
        Matrix result = Divide(left, right);
        return result;
    }

    public override int GetHashCode()
    {
        // numCells * (row*col) * cellValue
        int numCells = this.Row * this.Column;

        double hashCode = 0;

        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                hashCode += numCells * (row * col) * this[row, col];
            }
        }

        return (int)hashCode;
    }

    /// <summary>
    /// Element-wise comparison of two matries
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right matrix</param>
    /// <returns>true or false</returns>
    public static bool operator ==(Matrix left, Matrix right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (ReferenceEquals(left, null))
        {
            return false;
        }
        if (ReferenceEquals(right, null))
        {
            return false;
        }

        // check size
        if (left.Size != right.Size)
        {
            return false;
        }

        // check each value
        for (int row = 0; row < left.Row; row++)
        {
            for (int col = 0; col < left.Column; col++)
            {
                if (left[row, col] != right[row, col])
                {
                    return false;
                }
            }
        }

        // if all values are the same
        return true;
    }

    /// <summary>
    /// Element-wise comparison of two matries
    /// </summary>
    /// <param name="left">left matrix</param>
    /// <param name="right">right matrix</param>
    /// <returns>true or false</returns>
    public static bool operator !=(Matrix left, Matrix right)
    {
        return !(left == right);
    }

    #endregion

    #region Math Functions

    /// <summary>
    /// Sigmoid function, take Z, return number between 0 - 1
    /// </summary>
    /// <param name="Z">W.T * X + b</param>
    /// <returns>A, size is the same as input</returns>
    public static Matrix Sigmoid(Matrix Z)
    {
        Matrix A = new Matrix(Z.Shape);
        A = 1 / (1 + Matrix.Exp(-1 * Z));
        return A;
    }
    /// <summary>
    /// Returns the sum of the whole matrix as a 1 x 1 matrix
    /// </summary>
    /// <returns>1 X 1 Matrix</returns>
    public static Matrix Sum(Matrix matrix)
    {
        Matrix result = new Matrix(1, 1);
        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                result[0, 0] = result[0, 0] + matrix[row, col];
            }
        }
        return result;
    }

    /// <summary>
    /// Sum the matrix according to axis (row or column)
    /// </summary>
    /// <param name="matrix">the matrix to be operated</param>
    /// <param name="axis">0 or 1, row or column</param>
    /// <returns>sum</returns>
    public static Matrix Sum(Matrix matrix, int axis)
    {
        Matrix result;

        // 0 2 4
        // 1 3 5
        // =====
        // 1 5 9
        if (axis == 0) // sums down the rows 
        {
            result = new Matrix(1, matrix.Column);
            Matrix column;
            for (int col = 0; col < matrix.Column; col++)
            {
                column = matrix.GetColumn(col);
                double rowSum = Matrix.Sum(column)[0];
                result[0, col] = rowSum;
            }
            return result;
        }
        else if (axis == 1) // sums across the columns
        {
            result = new Matrix(matrix.Row, 1);
            Matrix row;
            for (int rowIndex = 0; rowIndex < matrix.Row; rowIndex++)
            {
                row = matrix.GetRow(rowIndex);
                double colSum = Matrix.Sum(row)[0];
                result[rowIndex, 0] = colSum;
            }
            return result;
        }
        else
        {
            throw new ArgumentException("Axis can only be 1 or 2, row or column");
        }
    }

    /// <summary>
    /// element wise, turn all number into absolute number. Size remain unchanged
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Matrix Abs(Matrix matrix)
    {
        Matrix result = new Matrix(matrix.Shape);
        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                result[row, col] = Math.Abs(matrix[row, col]);
            }
        }
        return result;
    }

    /// <summary>
    /// element-wise power
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="num">the specific num to be raised to</param>
    /// <returns></returns>
    public static Matrix Power(Matrix matrix, double num)
    {
        Matrix result = new Matrix(matrix.Shape);
        for (int row = 0; row < result.Row; row++)
        {
            for (int col = 0; col < result.Column; col++)
            {
                result[row, col] = Math.Pow(matrix[row, col], num);
            }
        }
        return result;
    }

    /// <summary>
    /// Calculates the mean of the matrix
    /// </summary>
    /// <param name="matrix">the matrix to be calculated</param>
    /// <returns></returns>
    public static Matrix Mean(Matrix matrix)
    {
        double sum = 0;
        double n = matrix.Row * matrix.Column;
        double mean;
        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                sum = sum + matrix[row, col];
            }
        }
        mean = sum / n;
        Matrix result = new Matrix(1, 1).SetNum(mean);

        return result;
    }

    /// <summary>
    /// element-wise Exp, e^matrix
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns>a matrix after calculation</returns>
    public static Matrix Exp(Matrix matrix)
    {
        Matrix newMatrix = new Matrix(matrix.Shape);

        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                newMatrix[row, col] = Math.Exp(matrix[row, col]);//e^num
            }
        }
        return newMatrix;
    }

    /// <summary>
    /// element-wise natural log (base e)
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static Matrix Log(Matrix matrix)
    {

        Matrix newMatrix = new Matrix(matrix.Shape);

        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                newMatrix[row, col] = Math.Log(matrix[row, col]);
            }
        }
        return newMatrix;
    }

    /// <summary>
    /// element-wise tanh, (e^z - e^-z)/(e^z + e^-z)
    /// </summary>
    /// <param name="Z"></param>
    /// <returns>a matrix which has the same dimension as input</returns>
    public static Matrix tanh(Matrix Z)
    {
        Matrix result = new Matrix(Z.Shape);
        result = (Matrix.Exp(Z) - Matrix.Exp(-1 * Z)) / (Matrix.Exp(Z) + Matrix.Exp(-1 * Z));
        return result;
    }

    /// <summary>
    /// Uses the kernel to 'slides' across the input matrix
    /// </summary>
    /// <param name="input">the input matrix</param>
    /// <param name="kernel">a squared kernel</param>
    /// <returns>the matrix after convolution</returns>
    public static Matrix Convolve(Matrix input, Matrix kernel)
    {
        // create a matrix with extra row and col
        int extraLength = kernel.Row / 2;
        int kernelLength = kernel.Row;

        Matrix extendedMatrix = new Matrix(extraLength + input.Row + extraLength,
                                           extraLength + input.Column + extraLength);

        // fill the extended matrix using the input matrix
        for (int row = 0; row < input.Row; row++)
        {
            for (int col = 0; col < input.Column; col++)
            {
                extendedMatrix[row + extraLength, col + extraLength] = input[row, col];
            }
        }

        // create the output matrix which has the same size as input matrix
        Matrix output = new Matrix(input.Shape);

        // fill the output matrix by sliding over the input matrix
        Matrix square = new Matrix(kernel.Shape);
        for (int row = 0; row < output.Row; row++)
        {
            for (int col = 0; col < output.Column; col++)
            {
                // fill the square using the extended matrix
                for (int i = 0; i < kernelLength; i++)
                {
                    for (int j = 0; j < kernelLength; j++)
                    {
                        square[i, j] = extendedMatrix[row + i, col + j];
                    }
                }
                output[row, col] = Matrix.Sum(square * kernel)[0];
            }
        }
        return output;
    }

    #endregion

    #region Matrix Non-Static Methods

    /// <summary>
    /// Gets the matrix as a double[,]
    /// </summary>
    /// <returns>a double[,] containing matrix values</returns>
    public double[,] ToDoubleArray()
    {
        return this._data;
    }

    /// <summary>
    /// Gets the matrix as a byte[,]
    /// </summary>
    /// <returns>a byte[,] containing matrix values</returns>
    public byte[,] ToByteArray()
    {
        return Matrix.ToByteArray(this);
    }

    /// <summary>
    /// Sets all elements to a specific nummber
    /// </summary>
    /// <param name="num">the number to set</param>
    /// <returns>a matrix which is full of the number</returns>
    public Matrix SetNum(double num)
    {
        Matrix newMatrix = new Matrix(this.Shape);
        for (int row = 0; row < newMatrix.Row; row++)
        {
            for (int col = 0; col < newMatrix.Column; col++)
            {
                newMatrix[row, col] = num;
            }
        }
        return newMatrix;
    }

    /// <summary>
    /// Reshapes the matrix using row and column
    /// </summary>
    /// <param name="row">the number of rows</param>
    /// <param name="col">the number of columns</param>
    /// <returns>return a row x col matrix</returns>   
    public Matrix Reshape(int row, int col)
    {
        if (row * col != this.Row * this.Column)
        {
            Console.WriteLine("cannot reshpe this matrix");
            Console.WriteLine($"Orginal: {this.Row} X {this.Column} != Output: {row} X {col}");
            return this;
        }
        // create a list to store original matrix values (each cell)
        List<double> originalValues = new List<double>();
        for (int originalRow = 0; originalRow < this.Row; originalRow++)
        {
            for (int originalCol = 0; originalCol < this.Column; originalCol++)
            {
                originalValues.Add(this[originalRow, originalCol]);
            }
        }

        Matrix shappedMatrix;
        shappedMatrix = new Matrix(row, col);
        int listIndex = 0;

        for (int shappedRow = 0; shappedRow < shappedMatrix.Row; shappedRow++)
        {
            for (int shappedCol = 0; shappedCol < shappedMatrix.Column; shappedCol++)
            {
                shappedMatrix[shappedRow, shappedCol] = originalValues[listIndex];
                listIndex++;
            }
        }
        return shappedMatrix;
    }

    /// <summary>
    /// Reshapes the matrix using a 1D int array
    /// </summary>
    /// <param name="shape">1D array containing row and col</param>
    /// <returns></returns>
    public Matrix Reshape(int[] shape)
    {
        int row = shape[0];
        int col = shape[1];
        return this.Reshape(row,col);
    }
    
    /// <summary>
    /// Reshapes the matrix using column number only
    /// </summary>
    /// <param name="col">the number of columns</param>
    /// <returns>a reshaped matrix</returns>
    public Matrix Reshape(int col)
    {
        if (((this.Row * this.Column) % col) != 0)
        {
            // e.g. some reshpre might cause this
            // {1,2,3,4,5},
            // {6,7,8,9  } ---- miss one element
            throw new ArgumentException("The matrix cannot be perfectly reshaped");
        }
        int row = (this.Row * this.Column) / col;
        return this.Reshape(row, col);
    }

    /// <summary>
    /// Returns a string conatining "Row X Column"
    /// </summary>
    /// <returns>"Row X Column"</returns>
    public override string ToString()
    {
        string result;

        result = $"Row X Column : {this.Row} X {this.Column}";

        return result;
    }

    /// <summary>
    /// Display the matrix
    /// </summary>
    /// <param name="numDecimals">the number of decimal spaces to use</param>
    public void Display(int numDecimals = 2)
    {
        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                Console.Write(Math.Round(this[row, col], numDecimals) + "\t");
            }
            Console.WriteLine();
        }
        Console.WriteLine($"\nThis is a {this.Row} x {this.Column} Matrix");
    }

    /// <summary>
    /// Converts the whole matrix into a string, that can be directly save as a text file
    /// </summary>
    /// <returns>a string containing the whole matrix</returns>
    public string ReturnString()
    {
        string text = "";
        for (int row = 0; row < this.Row; row++)
        {
            text = text + "{";
            for (int col = 0; col < this.Column; col++)
            {
                if (col == this.Column - 1)
                {
                    text = text + (this[row, col]);
                }
                else
                {
                    text = text + (this[row, col] + ",");
                }
            }
            text = text + "},\n";
        }
        return text;
    }

    /// <summary>
    /// Gets the specific column as a new matrix (one column)
    /// </summary>
    /// <param name="colIndex">the specific column</param>
    /// <returns>one column matrix</returns>
    public Matrix GetColumn(int colIndex)
    {
        Matrix column = new Matrix(this.Row, 1);
        for (int row = 0; row < this.Row; row++)
        {
            column[row, 0] = this[row, colIndex];
        }
        return column;
    }

    /// <summary>
    /// Gets the specific row as a new matrix (one row)
    /// </summary>
    /// <param name="rowIndex">the index of the row</param>
    /// <returns>one row matrix</returns>
    public Matrix GetRow(int rowIndex)
    {
        Matrix row = new Matrix(1, this.Column);
        for (int col = 0; col < this.Column; col++)
        {
            row[0, col] = this[rowIndex, col];
        }
        return row;
    }

    /// <summary>
    /// Removes a specific column in the matrix
    /// </summary>
    /// <param name="colIndex">the column's index to be removed</param>
    /// <returns>return a new matrix after removing</returns>
    public Matrix RemoveColumn(int colIndex)
    {
        Matrix result;

        if (colIndex == 0)
        {
            result = new Matrix(this.Row, this.Column - 1);

            for (int row = 0; row < result.Row; row++)
            {
                for (int col = 0; col < result.Column; col++)
                {
                    result[row, col] = this[row, col + 1];
                }
            }
            return result;
        }
        // deal with col index != 0
        Matrix leftMatrix = new Matrix(this.Row, colIndex);
        Matrix rightMatrix = new Matrix(this.Row, this.Column - colIndex - 1);

        // populate the left matrix
        for (int row = 0; row < leftMatrix.Row; row++)
        {
            for (int col = 0; col < leftMatrix.Column; col++)
            {
                leftMatrix[row, col] = this[row, col];
            }
        }

        // populate the right matrix
        for (int row = 0; row < rightMatrix.Row; row++)
        {
            for (int col = 0; col < rightMatrix.Column; col++)
            {
                rightMatrix[row, col] = this[row, col + colIndex + 1];
            }
        }

        // combine left and right
        result = leftMatrix.Concatenate(rightMatrix);
        return result;
    }

    /// <summary>
    /// Removes a range of columns based on the number given
    /// </summary>
    /// <param name="colIndex">start index</param>
    /// <param name="numColToBeRemoved">number of columns to be removed</param>
    /// <returns></returns>
    public Matrix RemoveColumn(int colIndex, int numColToBeRemoved)
    {
        // new col  = orgiranl col - num_of_cols to be removed
        Matrix matrix = new Matrix(this);
        int numColRemoved = 0;
        while (true)
        {
            matrix = matrix.RemoveColumn(colIndex);
            numColRemoved++;
            if (numColRemoved == numColToBeRemoved) { break; }
        }
        return matrix;
    }

    /// <summary>
    /// Removes a specific row in the matrix
    /// </summary>
    /// <param name="rowIndex">the index of the row to be removed</param>
    /// <returns>return a new matrix after removing</returns>
    public Matrix RemoveRow(int rowIndex)
    {
        Matrix result;

        if (rowIndex == 0)
        {
            result = new Matrix(this.Row - 1, this.Column);

            // populate the result
            for (int row = 0; row < result.Row; row++)
            {
                for (int col = 0; col < result.Column; col++)
                {
                    result[row, col] = this[row + 1, col];
                }
            }
        }

        // deal with row_index != 0
        Matrix topMatrix = new Matrix(rowIndex, this.Column);
        Matrix bottomMatrix = new Matrix(this.Row - rowIndex - 1, this.Column);

        // populate the top matrix
        for (int row = 0; row < topMatrix.Row; row++)
        {
            for (int col = 0; col < topMatrix.Column; col++)
            {
                topMatrix[row, col] = this[row, col];
            }
        }

        // populate the right matrix
        for (int row = 0; row < bottomMatrix.Row; row++)
        {
            for (int col = 0; col < bottomMatrix.Column; col++)
            {
                bottomMatrix[row, col] = this[row + rowIndex + 1, col];
            }
        }

        // combine top and bootom
        result = topMatrix.BottomConcatenate(bottomMatrix);

        return result;

    }

    /// <summary>
    /// Removes a range of rows in the matrix
    /// </summary>
    /// <param name="rowIndex">start index</param>
    /// <param name="numRows">how many rows to be removed</param>
    /// <returns>returns a new matrix after removing</returns>
    public Matrix RemoveRow(int rowIndex, int numRows)
    {
        // new row  = orgiranl row - num_of_rows to be removed
        Matrix newMatrix = new Matrix(this);
        int rowRemoved = 0;
        while (true)
        {
            newMatrix = newMatrix.RemoveRow(rowIndex);
            rowRemoved++;
            if (rowRemoved == numRows) { break; }
        }
        return newMatrix;
    }
    /// <summary>
    /// Concatenates two matries together,left to right
    /// </summary>
    /// <param name="right">the matrix to be combined</param>
    /// <returns>return the combined matrix, horizontally</returns>
    public Matrix Concatenate(Matrix right)
    {
        // check row number
        if (this.Row != right.Row)
        {
            throw new ArgumentException($"{this.Row}!={right.Row}\n row number has to be the same");
        }
        Matrix newMatrix = new Matrix(this.Row, this.Column + right.Column);
        // populate the new matrix by using the left matrix
        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                newMatrix[row, col] = this[row, col];
            }
        }

        // using the right matrix
        for (int row = 0; row < right.Row; row++)
        {
            for (int col = this.Column; col < this.Column + right.Column; col++)
            {
                newMatrix[row, col] = right[row, col - this.Column];
            }
        }
        return newMatrix;

    }

    /// <summary>
    /// Concatenates the matrix together, top to bottom
    /// </summary>
    /// <param name="bottom">the matrix to be concatenated from bottom</param>
    /// <returns>a taller matrix, vertically</returns>
    public Matrix BottomConcatenate(Matrix bottom)
    {

        // check column number
        if (this.Column != bottom.Column)
        {
            throw new ArgumentException($"{this.Column}!={bottom.Column}\n both column number has to be the same");
        }

        // there will be some extra rows depends on the bottom matrix's row
        Matrix newMatrix = new Matrix(this.Row + bottom.Row, this.Column);

        // populate the new matrix by using the up (original) matrix
        for (int row = 0; row < this.Row; row++)
        {
            for (int col = 0; col < this.Column; col++)
            {
                newMatrix[row, col] = this[row, col];
            }
        }

        // populate the extra part with the bottom matrix by iterating over the bottom matrix
        for (int row = 0; row < bottom.Row; row++)
        {
            for (int col = 0; col < bottom.Column; col++)
            {
                newMatrix[row + this.Row, col] = bottom[row, col];
            }
        }
        return newMatrix;
    }


    /// <summary>
    /// Saves the matrix in a file
    /// </summary>
    /// <param name="filePath">the file path</param>
    public void SaveMatrix(string filePath)
    {
        bool isDataFile = CheckFileExtension(filePath, "data");
        bool isTextFile = CheckFileExtension(filePath, "txt");

        if (isDataFile)
        {
            Save2DArray(this._data, filePath);
        }
        else if (isTextFile)
        {
            SaveMatrixText(this, filePath);
        }
    }


    #endregion

    #region Matrix Static Methods

    /// <summary>
    /// Gets the max value of the column according to the given index
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="colIndex"></param>
    /// <returns>a matrix(1x1) which is the maximum value of the specific column</returns>
    public static Matrix GetMax(Matrix matrix, int colIndex)
    {
        double maxValue = double.MinValue;
        int maxIndex = 0;
        for (int row = 0; row < matrix.Row; row++)
        {
            if (matrix[row, colIndex] > maxValue)
            {
                maxValue = matrix[row, colIndex];
                maxIndex = row;
            }
        }
        Matrix result = new Matrix(1, 1).SetNum(maxValue);

        return result;
    }

    /// <summary>
    /// Find the index with max value in each column and turn into 1 row matrix
    /// </summary>
    /// <param name="matrix">The matrix to be searched</param>
    /// <returns>Returns an one row matrix</returns>
    public static Matrix GetMax(Matrix matrix)
    {
        const int NUM_ROWS_TO_RETURN = 1;

        // the number of column is the same as the input's column
        Matrix result = new Matrix(NUM_ROWS_TO_RETURN, matrix.Column);

        // fill the each column

        for (int col = 0; col < result.Column; col++)
        {
            result[0, col] = Matrix.GetMax(matrix, col)[0];
        }

        return result;
    }

    /// <summary>
    /// Converts the matrix into a 2D byte array
    /// </summary>
    /// <param name="matrix">the matrix to be converted</param>
    /// <returns>a 2D byte array</returns>
    public static byte[,] ToByteArray(Matrix matrix)
    {
        byte[,] result = new byte[matrix.Row, matrix.Column];
        for (int row = 0; row < matrix.Row; row++)
        {
            for (int col = 0; col < matrix.Column; col++)
            {
                if (matrix[row, col] > 255)
                {
                    result[row, col] = 255;
                }
                else if (matrix[row, col] < 0)
                {
                    result[row, col] = 0;
                }
                else
                {
                    result[row, col] = (byte)matrix[row, col];
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Concatenates two matries from left to right
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns>leftright</returns>
    public static Matrix Concatenate(Matrix left, Matrix right)
    {
        return left.Concatenate(right);
    }
    
    #endregion

    #region Helper Methods

    // ===========For Load and Save matrix===========
    private static byte[] DoubleArrToBytes(double[,] arr)
    {
        byte[] bytes = new byte[arr.GetLength(0) * arr.GetLength(1) * 8];

        for (int i = 0; i < bytes.Length; i += 8)
        {
            int id = i / 8;
            int row = id / arr.GetLength(1);
            int col = id % arr.GetLength(1);

            byte[] b = BitConverter.GetBytes(arr[row, col]);
            for (int j = i; j < i + 8; j++)
            {
                bytes[j] = b[j - i];
            }
        }
        return bytes;
    }
    private static void Save2DArray(double[,] arr, string path)
    {
        byte[] dim1Bytes = BitConverter.GetBytes((double)arr.GetLength(0));
        byte[] dim2Bytes = BitConverter.GetBytes((double)arr.GetLength(1));
        byte[] dataBytes = DoubleArrToBytes(arr);
        byte[] allBytes = new byte[dim1Bytes.Length + dim2Bytes.Length + dataBytes.Length];
        Array.Copy(dim1Bytes, 0, allBytes, 0, dim1Bytes.Length);
        Array.Copy(dim2Bytes, 0, allBytes, 8, dim2Bytes.Length);
        Array.Copy(dataBytes, 0, allBytes, 16, dataBytes.Length);
        System.IO.File.WriteAllBytes(path, allBytes);
    }
    private static double[] ByteArrToDouble(byte[] bytes)
    {
        double[] arr = new double[bytes.Length / 8];
        for (int i = 0; i < bytes.Length; i += 8)
        {
            arr[i / 8] = BitConverter.ToDouble(bytes, i);
        }
        return arr;
    }
    private static double[,] Load2DArray(string path)
    {
        byte[] allBytes = System.IO.File.ReadAllBytes(path);
        byte[] dataBytes = new byte[allBytes.Length - 16];
        Array.Copy(allBytes, 16, dataBytes, 0, dataBytes.Length);

        double dim1 = BitConverter.ToDouble(allBytes, 0);
        double dim2 = BitConverter.ToDouble(allBytes, 8);

        double[] arr1D = ByteArrToDouble(dataBytes);
        double[,] arr2D = new double[(int)dim1, (int)dim2];
        for (int row = 0; row < arr2D.GetLength(0); row++)
        {
            for (int col = 0; col < arr2D.GetLength(1); col++)
            {
                // Changes GetLength() from 0 to 1
                arr2D[row, col] = arr1D[row * arr2D.GetLength(1) + col];
            }
        }
        return arr2D;
    }

    // ===========For Load and Save matrix===========

    /// <summary>
    /// Checks whether the file matches the extension or not
    /// </summary>
    /// <param name="filePath">the file path</param>
    /// <param name="extension">file extension</param>
    /// <returns>true if matches</returns>
    public static bool CheckFileExtension(string filePath, string extension)
    {
        if (filePath.Substring(filePath.Length - $".{extension}".Length, $".{extension}".Length) == $".{extension}")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Saves the matrix as a text file
    /// </summary>
    /// <param name="matrix">the matrix to be saved</param>
    /// <param name="filePath">the file path to save the matrix</param>
    private static void SaveMatrixText(Matrix matrix, string filePath)
    {
        char delimiter = ',';
        string[] content = new string[matrix.Row];

        // Fills the content array
        for (int row = 0; row < matrix.Row; row++)
        {
            // Generates each row
            string rowData = "";
            for (int col = 0; col < matrix.Column; col++)
            {
                rowData += $"{matrix[row, col]}{delimiter}";
            }
            // Removes the delimiter at the end of each row
            content[row] = rowData.Substring(0, rowData.Length - 1);
        }

        System.IO.File.WriteAllLines(filePath, content);
    }
    #endregion
}

