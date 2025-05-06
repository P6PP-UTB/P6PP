export interface Booking {
    id: number,
    serviceId: number,
    userId: number,
    status: string
}

export interface BookingResponse {
    data: [Booking],
    success: boolean,
    message: string|null
}