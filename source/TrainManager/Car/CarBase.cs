﻿using LibRender2.Camera;
using LibRender2.Cameras;
using LibRender2.Trains;
using OpenBveApi.Graphics;
using OpenBveApi.Math;
using OpenBveApi.Routes;
using OpenBveApi.Trains;
using TrainManager.BrakeSystems;
using TrainManager.Power;

namespace TrainManager.Car
{
	/*
	 * TEMPORARY NAME AND CLASS TO ALLOW FOR MOVE IN PARTS
	 */
	public abstract class CarBase : AbstractCar
	{
		/// <summary>The front bogie</summary>
		public Bogie FrontBogie;
		/// <summary>The rear bogie</summary>
		public Bogie RearBogie;
		/// <summary>The doors for this car</summary>
		public Door[] Doors;
		/// <summary>Contains the physics properties for the car</summary>
		public CarPhysics Specs;
		/// <summary>The car brake for this car</summary>
		public CarBrake CarBrake;
		/// <summary>The car sections (objects) attached to the car</summary>
		public CarSection[] CarSections;
		/// <summary>The index of the current car section</summary>
		public int CurrentCarSection;
		/// <summary>The driver's eye position within the car</summary>
		public Vector3 Driver;
		/// <summary>The current yaw of the driver's eyes</summary>
		public double DriverYaw;
		/// <summary>The current pitch of the driver's eyes</summary>
		public double DriverPitch;
		/// <summary>Whether currently visible from the in-game camera location</summary>
		public bool CurrentlyVisible;
		/// <summary>Whether currently derailed</summary>
		public bool Derailed;
		/// <summary>Whether currently toppled over</summary>
		public bool Topples;
		/// <summary>The coupler between cars</summary>
		public Coupler Coupler;
		/// <summary>The breaker</summary>
		public Breaker Breaker;
		/// <summary>The windscreen</summary>
		public Windscreen Windscreen;
		/// <summary>The position of the beacon reciever within the car</summary>
		public double BeaconReceiverPosition;
		/// <summary>The beacon reciever</summary>
		public TrackFollower BeaconReceiver;
		/// <summary>Stores the camera restriction mode for the interior view of this car</summary>
		public CameraRestrictionMode CameraRestrictionMode = CameraRestrictionMode.NotSpecified;
		/// <summary>The current camera restriction mode for this car</summary>
		public CameraRestriction CameraRestriction;
		/// <summary>Stores the camera interior camera alignment for this car</summary>
		public CameraAlignment InteriorCamera;
	}
}
