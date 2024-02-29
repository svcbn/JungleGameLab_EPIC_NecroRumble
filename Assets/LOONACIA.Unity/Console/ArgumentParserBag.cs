using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LOONACIA.Unity.Console
{
	public static class ArgumentParserBag
	{
		public static bool TryGetVector3(ReadOnlySpan<char> parameter, out Vector3 vector)
		{
			bool isHandled = false;
			vector = Vector3.zero;

			var enumerator = parameter.Split(',');
			if (!enumerator.MoveNext() || enumerator.Current.Chars.SequenceEqual(parameter))
			{
				enumerator = parameter.Split(' ');
			}
			enumerator.Reset();

			foreach (var split in enumerator)
			{
				if (!float.TryParse(split.Chars, out var value))
				{
					continue;
				}

				switch (split.Index)
				{
					case 0:
						vector.x = value;
						isHandled = true;
						break;
					case 1:
						vector.y = value;
						isHandled = true;
						break;
					case 2:
						vector.z = value;
						isHandled = true;
						break;
				}
			}

			return isHandled;
		}

		public static bool TryGetGameObject(ReadOnlySpan<char> parameter, out GameObject gameObject)
		{
			string name = Regex.Match(parameter.ToString(), @"\w+|""[\w\s]*""").Value.Trim('\"');
			gameObject = GameObject.Find(name);
			return gameObject != null;
		}

        public static bool TryParseTwoIntegers(ReadOnlySpan<char> input, out int arg1, out int arg2)
        {
            arg1 = 0;
            arg2 = 0;

            // 공백을 기준으로 입력을 두 부분으로 나눕니다.
            int spaceIndex = input.IndexOf(' ');
            if (spaceIndex == -1)
            {
                return false; // 입력에 공백이 없으면 실패
            }

            ReadOnlySpan<char> firstPart = input.Slice(0, spaceIndex);
            ReadOnlySpan<char> secondPart = input.Slice(spaceIndex + 1);

            // 두 부분을 각각 int로 변환합니다.
            return int.TryParse(firstPart, out arg1) && int.TryParse(secondPart, out arg2);
        }
	}
}