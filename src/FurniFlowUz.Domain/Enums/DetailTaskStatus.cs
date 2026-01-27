namespace FurniFlowUz.Domain.Enums;

public enum DetailTaskStatus
{
    Pending,       // Waiting for previous task
    Ready,         // Can start now
    InProgress,    // Employee is working
    Completed,     // Done, awaiting QC
    QCPassed,      // Quality control approved
    QCFailed,      // Needs rework
    Rework         // Being redone
}
