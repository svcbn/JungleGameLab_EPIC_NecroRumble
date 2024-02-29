using System;

namespace LOONACIA.Unity.Console
{
	public delegate bool DebugCommandParser<T,U>(ReadOnlySpan<char> chars, out T argument1, out U argument2);

	public class DebugCommand<T,U> : DebugCommandBase
	{
		private readonly Action<T,U> _execute;

		private readonly DebugCommandParser<T,U> _parser;

		public DebugCommand(string id, string description, string format, Action<T,U> execute, DebugCommandParser<T,U> parser = null) : base(id, description, format)
		{
			_execute = execute;
			_parser = parser;
		}

		public void Execute(T parameter1, U parameter2)
		{
			_execute?.Invoke(parameter1, parameter2);
		}

		public void Execute(ReadOnlySpan<char> parameter)
		{
			if (_parser == null)
			{
				UnityEngine.Debug.LogError($"Argument parser is null.");
				return;
			}

			if (_parser(parameter, out var argument1, out var argument2))
			{
				_execute?.Invoke(argument1, argument2);
			}
		}

		public override void Execute(object parameter = null)
		{
			if (parameter is string paramString)
			{
				Execute(paramString);
				return;
			}

			if (!TryGetCommandArgument(parameter, out T argument1, out U argument2))
			{
				UnityEngine.Debug.LogError($"Invalid argument: {parameter}, argument1 type is {typeof(T)}. argument2 type is {typeof(U)}.");
			}

			Execute(argument1, argument2);
		}

		private static bool TryGetCommandArgument(object parameter, out T argument1, out U argument2)
		{
			if (parameter is Tuple<T, U> tuple)
			{
				argument1 = tuple.Item1;
				argument2 = tuple.Item2;
				return true;
			}

			// 기본값으로 설정
			argument1 = default;
			argument2 = default;

			// T 타입과 U 타입이 모두 null 가능한 경우
			if (parameter == null && default(T) == null && default(U) == null)
			{
				return true;
			}

			// T 타입 또는 U 타입이 아닌 경우 오류 반환
			if (!(parameter is T) && !(parameter is U))
			{
				UnityEngine.Debug.LogError($"Invalid argument type: {parameter?.GetType().Name}. Expected types are {typeof(T)} and {typeof(U)}.");
				return false;
			}

			// T 타입 또는 U 타입의 경우 해당 타입으로 설정
			if (parameter is T t)
			{
				argument1 = t;
				return true;
			}

			if (parameter is U u)
			{
				argument2 = u;
				return true;
			}

			return false;
		}
	}
}