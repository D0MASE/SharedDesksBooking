import React from "react";
import { ProfileData } from "../types";

interface ProfileViewProps {
    profileData: ProfileData | null;
}

const ProfileView: React.FC<ProfileViewProps> = ({ profileData }) => {
    if (!profileData) {
        return (
            <div className="text-center mt-5">
                <div className="spinner-border text-primary" role="status"></div>
                <p className="mt-2">Loading your profile...</p>
            </div>
        );
    }

    return (
        <div className="card shadow-sm p-4 animate-fade-in bg-white">
            <h3 className="text-center mb-4 border-bottom pb-3">
                {profileData?.firstName} {profileData?.lastName}'s History
            </h3>
            <div className="row">
                <div className="col-md-6">
                    <h5 className="text-success border-bottom pb-2">Active Reservations</h5>
                    {profileData?.currentReservations && profileData.currentReservations.length > 0 ? (
                        <div className="list-group">
                            {profileData.currentReservations.map((res) => (
                                <div key={res.id} className="list-group-item border-start-0 border-end-0">
                                    <strong>Desk {res.deskNumber}</strong>
                                    <div className="text-muted small">
                                        {new Date(res.startDate).toLocaleDateString()} -{" "}
                                        {new Date(res.endDate).toLocaleDateString()}
                                    </div>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <p className="text-muted">No active bookings found.</p>
                    )}
                </div>

                <div className="col-md-6 mt-4 mt-md-0">
                    <h5 className="text-secondary border-bottom pb-2">Past History</h5>
                    {profileData?.pastReservations && profileData.pastReservations.length > 0 ? (
                        <div className="list-group">
                            {profileData.pastReservations.map((res) => (
                                <div key={res.id} className="list-group-item list-group-item-light border-start-0 border-end-0">
                                    <strong>Desk {res.deskNumber}</strong>
                                    <div className="text-muted small">
                                        {new Date(res.startDate).toLocaleDateString()} -{" "}
                                        {new Date(res.endDate).toLocaleDateString()}
                                    </div>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <p className="text-muted">No past reservations found.</p>
                    )}
                </div>
            </div>
        </div>
    );
};

export default ProfileView;
