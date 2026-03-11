export interface Reservation {
    id: number;
    firstName: string;
    lastName: string;
    startDate: string;
    endDate: string;
    deskId: number;
}

export interface Desk {
    id: number;
    number: string;
    status: "Available" | "UnderMaintenance";
    reservation: Reservation | null;
}

export interface UserReservation {
    id: number;
    startDate: string;
    endDate: string;
    deskId: number;
    deskNumber: string;
}

export interface ProfileData {
    firstName: string;
    lastName: string;
    currentReservations: UserReservation[];
    pastReservations: UserReservation[];
}

export interface User {
    firstName: string;
    lastName: string;
}
