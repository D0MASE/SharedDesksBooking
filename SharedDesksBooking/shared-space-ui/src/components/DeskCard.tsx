import React from "react";
import DatePicker from "react-datepicker";
import { Desk, User } from "../types";

interface DeskCardProps {
    desk: Desk;
    user: User;
    bookingDeskId: number | null;
    startDate: Date;
    endDate: Date | null;
    onCancel: (reservationId: number, onlyToday: boolean) => void;
    onReserve: (deskId: number) => void;
    onSetBookingDeskId: (deskId: number | null) => void;
    onSetStartDate: (date: Date) => void;
    onSetEndDate: (date: Date | null) => void;
}

const DeskCard: React.FC<DeskCardProps> = ({
    desk,
    user,
    bookingDeskId,
    startDate,
    endDate,
    onCancel,
    onReserve,
    onSetBookingDeskId,
    onSetStartDate,
    onSetEndDate,
}) => {
    const statusClass =
        desk.status === "UnderMaintenance"
            ? "secondary"
            : desk.reservation
                ? "danger"
                : "success";

    const isMine =
        desk.reservation &&
        desk.reservation.firstName.toLowerCase() === user.firstName.toLowerCase() &&
        desk.reservation.lastName.toLowerCase() === user.lastName.toLowerCase();

    return (
        <div className="col-6 col-md-4 col-lg-3 text-center">
            <div
                className={`card h-100 border-${statusClass} shadow-sm`}
                title={
                    desk.reservation
                        ? `Reserved by: ${desk.reservation.firstName} ${desk.reservation.lastName}`
                        : desk.status === "UnderMaintenance"
                            ? "Under Maintenance"
                            : "Available"
                }
            >
                <div className={`card-header bg-${statusClass} text-white fw-bold py-3`}>
                    Desk {desk.number}
                </div>

                <div className="card-body d-flex flex-column align-items-center justify-content-center py-4">
                    {isMine && desk.reservation ? (
                        <div className="d-grid gap-2 w-100">
                            <small className="fw-bold text-danger text-center">Your Booking</small>
                            <button
                                onClick={() => onCancel(desk.reservation!.id, true)}
                                className="btn btn-danger btn-sm"
                            >
                                Cancel today
                            </button>
                            <button
                                onClick={() => onCancel(desk.reservation!.id, false)}
                                className="btn btn-outline-danger btn-sm"
                            >
                                Cancel all
                            </button>
                        </div>
                    ) : !desk.reservation && desk.status !== "UnderMaintenance" ? (
                        bookingDeskId === desk.id ? (
                            <div className="w-100">
                                <small className="fw-bold mb-1 d-block">Select Range:</small>
                                <DatePicker
                                    selected={startDate}
                                    onChange={(dates: [Date | null, Date | null]) => {
                                        const [start, end] = dates;
                                        if (start) onSetStartDate(start);
                                        onSetEndDate(end);
                                    }}
                                    startDate={startDate}
                                    endDate={endDate || undefined}
                                    selectsRange
                                    inline
                                    minDate={new Date()}
                                />
                                <div className="d-flex gap-1 mt-2">
                                    <button
                                        onClick={() => onReserve(desk.id)}
                                        className="btn btn-success btn-sm w-100"
                                    >
                                        Confirm
                                    </button>
                                    <button
                                        onClick={() => onSetBookingDeskId(null)}
                                        className="btn btn-light btn-sm w-100 border"
                                    >
                                        Cancel
                                    </button>
                                </div>
                            </div>
                        ) : (
                            <button
                                onClick={() => {
                                    onSetBookingDeskId(desk.id);
                                    onSetStartDate(startDate); // Usually current selected date or today
                                    onSetEndDate(startDate);
                                }}
                                className="btn btn-success btn-sm px-4"
                            >
                                Reserve
                            </button>
                        )
                    ) : null}
                    {(desk.reservation && !isMine) || desk.status === "UnderMaintenance" ? (
                        <span className="text-muted small">
                            {desk.status === "UnderMaintenance"
                                ? "Under Maintenance"
                                : "Not Available"}
                        </span>
                    ) : null}
                </div>
            </div>
        </div>
    );
};

export default DeskCard;
