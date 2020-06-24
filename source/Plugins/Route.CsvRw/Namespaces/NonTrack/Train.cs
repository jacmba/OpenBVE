﻿using System;
using System.Collections.Generic;
using OpenBveApi;
using OpenBveApi.Interface;
using OpenBveApi.Math;
using OpenBveApi.Textures;

namespace CsvRwRouteParser
{
	internal partial class Parser
	{
		private void ParseTrainCommand(string Command, string[] Arguments, int Index, Expression Expression, ref RouteData Data, bool PreviewOnly)
		{
			switch (Command)
			{
				case "interval":
				{
					if (!PreviewOnly)
					{
						List<double> intervals = new List<double>();
						for (int k = 0; k < Arguments.Length; k++)
						{
							double o;
							if (!NumberFormats.TryParseDoubleVb6(Arguments[k], out o))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Interval " + k.ToString(Culture) + " is invalid in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								continue;
							}

							if (o == 0)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Interval " + k.ToString(Culture) + " must be non-zero in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								continue;
							}

							if (o > 43200 && Plugin.CurrentOptions.EnableBveTsHacks)
							{
								//Southern Blighton- Treston park has a runinterval of well over 24 hours, and there are likely others
								//Discard this
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Interval " + k.ToString(Culture) + " is greater than 12 hours in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								continue;
							}

							if (o < 120 && Plugin.CurrentOptions.EnableBveTsHacks)
							{
								/*
								 * An AI train follows the same schedule / rules as the player train
								 * ==>
								 * x Waiting time before departure at the first station (30s to 1min is 'normal')
								 * x Time to accelerate to linespeed
								 * x Time to clear (as a minimum) the protecting signal on station exit
								 *
								 * When the runinterval is below ~2minutes, on large numbers of routes, this
								 * shows up as a train overlapping the player train (bad....)
								 */
								o = 120;
							}

							intervals.Add(o);
						}

						intervals.Sort();
						if (intervals.Count > 0)
						{
							CurrentRoute.PrecedingTrainTimeDeltas = intervals.ToArray();
						}
					}
				}
					break;
				case "velocity":
				{
					if (!PreviewOnly)
					{
						double limit = 0.0;
						if (Arguments.Length >= 1 && Arguments[0].Length > 0 && !NumberFormats.TryParseDoubleVb6(Arguments[0], out limit))
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Speed is invalid in Train.Velocity at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							limit = 0.0;
						}

						Plugin.CurrentOptions.PrecedingTrainSpeedLimit = limit <= 0.0 ? double.PositiveInfinity : Data.UnitOfSpeed * limit;
					}
				}
					break;
				case "folder":
				case "file":
				{
					if (PreviewOnly)
					{
						if (Arguments.Length < 1)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FolderName contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								Plugin.CurrentOptions.TrainName = Arguments[0];
							}
						}
					}
				}
					break;
				case "run":
				case "rail":
				{
					if (!PreviewOnly)
					{
						if (Index < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RailTypeIndex is out of range in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							int val = 0;
							if (Arguments.Length >= 1 && Arguments[0].Length > 0 && !NumberFormats.TryParseIntVb6(Arguments[0], out val))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RunSoundIndex is invalid in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								val = 0;
							}

							if (val < 0)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RunSoundIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								val = 0;
							}

							if (Index >= Data.Structure.Run.Length)
							{
								Array.Resize(ref Data.Structure.Run, Index + 1);
							}

							Data.Structure.Run[Index] = val;
						}
					}
					else
					{
						railtypeCount++;
					}
				}
					break;
				case "flange":
				{
					if (!PreviewOnly)
					{
						if (Index < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "RailTypeIndex is out of range in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							int val = 0;
							if (Arguments.Length >= 1 && Arguments[0].Length > 0 && !NumberFormats.TryParseIntVb6(Arguments[0], out val))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FlangeSoundIndex is invalid in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								val = 0;
							}

							if (val < 0)
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FlangeSoundIndex expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								val = 0;
							}

							if (Index >= Data.Structure.Flange.Length)
							{
								Array.Resize(ref Data.Structure.Flange, Index + 1);
							}

							Data.Structure.Flange[Index] = val;
						}
					}
				}
					break;
				case "timetable.day":
				{
					if (!PreviewOnly)
					{
						if (Index < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "TimetableIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else if (Arguments.Length < 1)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								while (Index >= Data.TimetableDaytime.Length)
								{
									int n = Data.TimetableDaytime.Length;
									Array.Resize(ref Data.TimetableDaytime, n << 1);
									for (int i = n; i < Data.TimetableDaytime.Length; i++)
									{
										Data.TimetableDaytime[i] = null;
									}
								}

								string f = string.Empty;
								if (!string.IsNullOrEmpty(TrainPath))
								{
									f = Path.CombineFile(TrainPath, Arguments[0]);
								}
									
								if (!System.IO.File.Exists(f))
								{
									f = Path.CombineFile(ObjectPath, Arguments[0]);
								}

								if (System.IO.File.Exists(f))
								{
									Plugin.CurrentHost.RegisterTexture(f, new TextureParameters(null, null), out Data.TimetableDaytime[Index]);
								}
								else
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, false, "DaytimeTimetable " + Index + " with FileName " + Arguments[0] + " was not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
							}
						}
					}
				}
					break;
				case "timetable.night":
				{
					if (!PreviewOnly)
					{
						if (Index < 0)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, "TimetableIndex is expected to be non-negative in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else if (Arguments.Length < 1)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (Path.ContainsInvalidChars(Arguments[0]))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "FileName " + Arguments[0] + " contains illegal characters in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
							else
							{
								while (Index >= Data.TimetableNighttime.Length)
								{
									int n = Data.TimetableNighttime.Length;
									Array.Resize(ref Data.TimetableNighttime, n << 1);
									for (int i = n; i < Data.TimetableNighttime.Length; i++)
									{
										Data.TimetableNighttime[i] = null;
									}
								}

								string f = string.Empty;
								if(!string.IsNullOrEmpty(TrainPath))
								{
									f = Path.CombineFile(TrainPath, Arguments[0]);
								}

								if (!System.IO.File.Exists(f))
								{
									f = Path.CombineFile(ObjectPath, Arguments[0]);
								}

								if (System.IO.File.Exists(f))
								{
									Plugin.CurrentHost.RegisterTexture(f, new TextureParameters(null, null), out Data.TimetableNighttime[Index]);
								}
								else
								{
									Plugin.CurrentHost.AddMessage(MessageType.Error, false, "DaytimeTimetable " + Index + " with FileName " + Arguments[0] + " was not found in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
								}
							}
						}
					}
				}
					break;
				case "destination":
				{
					if (!PreviewOnly)
					{
						if (Arguments.Length < 1)
						{
							Plugin.CurrentHost.AddMessage(MessageType.Error, false, Command + " is expected to have one argument at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
						}
						else
						{
							if (!NumberFormats.TryParseIntVb6(Arguments[0], out Plugin.CurrentOptions.InitialDestination))
							{
								Plugin.CurrentHost.AddMessage(MessageType.Error, false, "Destination is expected to be an Integer in " + Command + " at line " + Expression.Line.ToString(Culture) + ", column " + Expression.Column.ToString(Culture) + " in file " + Expression.File);
							}
						}
					}
				}
					break;
			}
		}
	}
}
