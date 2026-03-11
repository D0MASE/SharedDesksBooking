import React from "react";
import DatePicker from "react-datepicker";
import { User } from "../types";
import { useDesks } from "../hooks/useDesks";
import IdentificationSection from "../components/IdentificationSection";
import DeskCard from "../components/DeskCard";

interface DesksPageProps {
    user: User;
    onUserChange: (user: User) => void;
}

const DesksPage: React.FC<DesksPageProps> = ({ user, onUserChange }) => {
    const {
        desks,
        selectedDate,
        setSelectedDate,
        bookingDeskId,
        setBookingDeskId,
        startDate,
        setStartDate,
        endDate,
        setEndDate,
        handleCancel,
        handleReserve,
    } = useDesks(new Date(), user);

    return (
        <div className="animate-fade-in">
            <h1 className="text-center mb-4">Shared Desks Booking</h1>

            <IdentificationSection user={user} onUserChange={onUserChange} />

            <div className="row justify-content-center mb-5">
                <div className="col-md-4 p-4 shadow-sm bg-light rounded text-center border">
                    <label className="form-label fw-bold d-block">Select View Date:</label>
                    <DatePicker
                        selected={selectedDate}
                        onChange={(date: Date | null) => date && setSelectedDate(date)}
                        dateFormat="yyyy-MM-dd"
                        className="form-control text-center"
                    />
                </div>
            </div>

            <div className="row g-4">
                {desks.map((desk) => (
                    <DeskCard
                        key={desk.id}
                        desk={desk}
                        user={user}
                        bookingDeskId={bookingDeskId}
                        startDate={startDate}
                        endDate={endDate}
                        onCancel={handleCancel}
                        onReserve={handleReserve}
                        onSetBookingDeskId={setBookingDeskId}
                        onSetStartDate={setStartDate}
                        onSetEndDate={setEndDate}
                    />
                ))}
            </div>
        </div>
    );
};

export default DesksPage;
