# Marlyn

![Demo Video](https://i.postimg.cc/FHG49TJ8/demo.gif)

A Chess simulator and engine built using the Unity game engine.

## Overview

The simulator was developed to learn the basics of Unity and get a feel for how C# scripts and code is organized. The project features a core game controller found in `Game.cs`, which handles all the UI related code. On the logic and game side, `Piece.cs`, `Move.cs`, and `Board.cs` makeup the code that works to determine move eligibility and patterns. `Board.cs` contains logic to determine the basic movement patterns for each type of piece, as well as functions to determine the check status of the game. The class can also get all the legal moves for a specific piece.

This project isn't being actively updated and serves mostly as a hosting repository for this experiment. Feel free to inspect the code or use the `Board` class, specifically, to assist you in creating your own Chess clone.

The project also contains a series of tests, which at time of writing all pass. They can be run in the Unity Test Editor.

## Specifications

- Unity Version: 2021.3.11f1
- Packages: TextMeshPro, Test Framework
