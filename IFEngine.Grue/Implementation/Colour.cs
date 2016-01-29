// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Colour.cs" company="Michael Greger">
//   Copyright (c) 2006-2012 Michael Greger. All rights reserved. Distributed under the Microsoft Reciprocal License (Ms-RL).
// </copyright>
// <summary>
//   Zmachine colours.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Zmachine
{
    /// <summary>
    /// Zmachine colours.
    /// </summary>
    internal struct ColorStruct
    {
        /// <summary>
        /// The alpha component.
        /// </summary>
        private readonly byte alphaComponent;

        /// <summary>
        /// The blue component.
        /// </summary>
        private readonly byte blueComponent;

        /// <summary>
        /// The green component.
        /// </summary>
        private readonly byte greenComponent;

        /// <summary>
        /// The red component.
        /// </summary>
        private readonly byte redComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorStruct"/> struct.
        /// </summary>
        /// <param name="red">
        /// The red component.
        /// </param>
        /// <param name="green">
        /// The green component.
        /// </param>
        /// <param name="blue">
        /// The blue component.
        /// </param>
        internal ColorStruct(byte red, byte green, byte blue) : this(byte.MaxValue, red, green, blue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorStruct"/> struct.
        /// </summary>
        /// <param name="alphaComponent">
        /// The reserved flags.
        /// </param>
        /// <param name="red">
        /// The red component.
        /// </param>
        /// <param name="green">
        /// The green component.
        /// </param>
        /// <param name="blue">
        /// The blue component.
        /// </param>
        private ColorStruct(byte alphaComponent, byte red, byte green, byte blue)
        {
            this.alphaComponent = alphaComponent;
            this.redComponent = red;
            this.greenComponent = green;
            this.blueComponent = blue;
        }

        /// <summary>
        /// Gets the colour black.
        /// </summary>
        /// <value>
        /// The colour black.
        /// </value>
        internal static ColorStruct Black
        {
            get
            {
                return new ColorStruct(0, 0, 0);
            }
        }

        /// <summary>
        /// Gets the colour blue.
        /// </summary>
        /// <value>
        /// The colour blue.
        /// </value>
        internal static ColorStruct Blue
        {
            get
            {
                return new ColorStruct(0, 104, 176);
            }
        }

        /// <summary>
        /// Gets the colour cyan.
        /// </summary>
        /// <value>
        /// The colour cyan.
        /// </value>
        internal static ColorStruct Cyan
        {
            get
            {
                return new ColorStruct(0, 232, 232);
            }
        }

        /// <summary>
        /// Gets the colour dark grey.
        /// </summary>
        /// <value>
        /// The colour dark grey.
        /// </value>
        internal static ColorStruct DarkGrey
        {
            get
            {
                return new ColorStruct(88, 88, 88);
            }
        }

        /// <summary>
        /// Gets the colour green.
        /// </summary>
        /// <value>
        /// The colour green.
        /// </value>
        internal static ColorStruct Green
        {
            get
            {
                return new ColorStruct(0, 208, 0);
            }
        }

        /// <summary>
        /// Gets the colour light grey.
        /// </summary>
        /// <value>
        /// The colour light grey.
        /// </value>
        internal static ColorStruct LightGrey
        {
            get
            {
                return new ColorStruct(176, 176, 176);
            }
        }

        /// <summary>
        /// Gets the colour magenta.
        /// </summary>
        /// <value>
        /// The colour magenta.
        /// </value>
        internal static ColorStruct Magenta
        {
            get
            {
                return new ColorStruct(248, 0, 248);
            }
        }

        /// <summary>
        /// Gets the colour medium grey.
        /// </summary>
        /// <value>
        /// The colour medium grey.
        /// </value>
        internal static ColorStruct MediumGrey
        {
            get
            {
                return new ColorStruct(136, 136, 136);
            }
        }

        /// <summary>
        /// Gets the colour red.
        /// </summary>
        /// <value>
        /// The colour red.
        /// </value>
        internal static ColorStruct Red
        {
            get
            {
                return new ColorStruct(232, 0, 0);
            }
        }

        /// <summary>
        /// Gets the transparent colour.
        /// </summary>
        /// <value>
        /// The transparent colour.
        /// </value>
        internal static ColorStruct Transparent
        {
            get
            {
                return new ColorStruct(255, 0, 0, 0);
            }
        }

        /// <summary>
        /// Gets the colour white.
        /// </summary>
        /// <value>
        /// The colour white.
        /// </value>
        internal static ColorStruct White
        {
            get
            {
                return new ColorStruct(248, 248, 248);
            }
        }

        /// <summary>
        /// Gets the colour yellow.
        /// </summary>
        /// <value>
        /// The colour yellow.
        /// </value>
        internal static ColorStruct Yellow
        {
            get
            {
                return new ColorStruct(232, 232, 0);
            }
        }

        /// <summary>
        /// Gets the blue component.
        /// </summary>
        /// <value>
        /// The blue component.
        /// </value>
        internal byte BlueComponent
        {
            get
            {
                return this.blueComponent;
            }
        }

        /// <summary>
        /// Gets the green component.
        /// </summary>
        /// <value>
        /// The green component.
        /// </value>
        internal byte GreenComponent
        {
            get
            {
                return this.greenComponent;
            }
        }

        /// <summary>
        /// Gets the red component.
        /// </summary>
        /// <value>
        /// The red component.
        /// </value>
        internal byte RedComponent
        {
            get
            {
                return this.redComponent;
            }
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="first">
        /// The first colour to compare.
        /// </param>
        /// <param name="second">
        /// The second colour to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two colours are equal.
        /// </returns>
        public static bool operator ==(ColorStruct first, ColorStruct second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="first">
        /// The first colour to compare.
        /// </param>
        /// <param name="second">
        /// The second colour to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two colours are not equal.
        /// </returns>
        public static bool operator !=(ColorStruct first, ColorStruct second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Blends two colours.
        /// </summary>
        /// <param name="color">
        /// The colour to add.
        /// </param>
        /// <param name="alpha">
        /// The alpha transparency.
        /// </param>
        /// <returns>
        /// The blended colour.
        /// </returns>
        internal ColorStruct Blend(ColorStruct color, byte alpha)
        {
            // todo: Handle transparent.
            var originalFactor = ~alpha;
            var r = (byte)(((this.redComponent * originalFactor) + (color.redComponent * alpha)) / byte.MaxValue);
            var g = (byte)(((this.greenComponent * originalFactor) + (color.greenComponent * alpha)) / byte.MaxValue);
            var b = (byte)(((this.blueComponent * originalFactor) + (color.blueComponent * alpha)) / byte.MaxValue);
            return new ColorStruct(r, g, b);
        }

        /// <summary>
        /// Determines whether two colours are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two colours are equal.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is ColorStruct && this.Equals((ColorStruct)obj);
        }

        /// <summary>
        /// Determines whether two colours are equal.
        /// </summary>
        /// <param name="color">
        /// The colour to compare.
        /// </param>
        /// <returns>
        /// A value indicating whether two colours are equal.
        /// </returns>
        internal bool Equals(ColorStruct color)
        {
            return this.redComponent == color.redComponent && this.greenComponent == color.greenComponent && this.blueComponent == color.blueComponent && this.alphaComponent == color.alphaComponent;
        }

        /// <summary>
        /// Calculates the hashcode.
        /// </summary>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public override int GetHashCode()
        {
            return (this.redComponent << 24) + (this.greenComponent << 16) + (this.blueComponent << 8) + this.alphaComponent;
        }
    }
}