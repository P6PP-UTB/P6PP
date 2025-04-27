export interface Course {
    id: number;
    trainerId: number;
    start: Date;
    end: Date;
    price: number;
    serviceName: string;
    currentCapacity: number;
    totalCapacity: number;
    roomName: string;
    isCancelled: boolean;
}