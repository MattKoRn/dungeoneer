# Introduction #

This page outlines the design philosophy for Dungeoneer and establishes its overall vision. Any questionable design decisions should be viewed first in the context of the guidelines established in this document; if any decisions still do not grok after that there was probably a good reason at the time.

The principles listed here are shown in the order of their importance: any decision that negatively impacts a higher-listed principle (to a large degree) while positively impacting a lower-listed principle would not be considered.


# Design Principles #

## Dungeoneer is a Screensaver ##

First and foremost, Dungeoneer is designed from the standpoint of being a screensaver in its primary mode of operation.  Judge it less on its virtues as a role-playing game and more on its virtues as a screensaver.

Why a screensaver? Screensavers are useful and occasionally entertaining programs that don't receive a lot of attention from programmers, yet most computer users probably run one.  I feel that creating an entertaining screensaver like this would be a nice project that can impact a lot of computers and their users. (As an aside, it is also a sneaky way to run a roguelike game while at work, possibly. c\_c)

What is a screensaver? Avoiding the technical definition, a screensaver is a program the operating system automatically invokes after a certain period of inactivity from the user. Generally, a screensaver will cover the user's entire screen and animate it with some form of activity to keep the monitor working.  Older CRT Monitors were vulnerable to screen burn-in where a static image displayed from the monitor for too long would damage the screen and cause a "ghost" of that image to be permanently visible.  Screensavers are designed to display an ever-changing image on the screen to combat this burn-in effect.

Screensavers are also non-interactive; as a matter of fact, if there is some form of user activity, a screensaver will generally terminate immediately (although occasionally it will ask for a password first.)

Dungeoneer is being designed as a screensaver, so there are certain design decisions that need to be made.  In particular, the non-interactivity of the screensaver mode means that there might need to be some interesting design decisions made of how to communicate certain information to the player, as well as decisions that need to be made to combat burn-in (although burn-in is much less of an issue with modern monitors.)

## Dungeoneer is Designed to be Played by a Borg ##

## Dungeoneer is a Roguelike ##

## Dungeoneer is a Roleplaying Game ##