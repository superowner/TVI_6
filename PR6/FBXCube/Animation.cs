using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace FBXCube
{
    class Animation
    {
        private Animation()
        {
        }

        public static Animation FromFile(string fileName)
        {
            Animation result = new Animation();

            return result;
        }
    }

    class Skeleton
    {
        private Skeleton()
        {
        }

        private enum ReadState
        {
            Begin, Undefined, Tag, Version, Units
        }

        private enum AngleMode
        {
            Degrees, Radians
        }

        public static Skeleton FromFile(string fileName)
        {
            Skeleton result = new Skeleton();
            StreamReader reader = new StreamReader(fileName);
            ReadState state = ReadState.Begin;
            bool endline;
            int line_index = 1;
            while (!reader.EndOfStream)
            {
                string[] line_tokens = reader.ReadLine().Split((' ')).Select(line => line.Trim()).ToArray();
                string token = "";
                endline = false;
                for (int t_index = 0; t_index < line_tokens.Length; t_index++ )
                {
                    token = line_tokens[t_index];
                    if (token == "")
                        continue;
                    if (token[0] == '#')
                      break;
                    if (endline)
                        throw new InvalidDataException(String.Format("Unexpected tokens near end of line {0}", line_index));
                    if (t_index == 0 && token[0] == ':')
                    {
                        switch (token)
                        {
                            case ":version":
                                state = ReadState.Version;
                                continue;
                            case ":units":
                                state = ReadState.Units;
                                endline = true;
                                continue;
                            default:
                                throw new InvalidDataException(String.Format("Unknown token {1} at line {0}", line_index, token));
                        }
                    }

                    switch (state)
                    {
                        case ReadState.Version:
                            float ver;
                            if (float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out ver))
                            {
                                result.version = ver;
                                endline = true;
                                state = ReadState.Undefined;
                            }
                            else
                                throw new InvalidDataException(String.Format("Invalid format at line {0}: {1} is not like a float number", line_index, token));

                            break;
                    }
                }
                line_index++;

            }

            return result;
        }

        private AngleMode angleMode;

        public float version { get; set; }
        public float part_mass { get; set; }
        public float part_length { get; set; }

        public string Name { get; set; }
    }
}
