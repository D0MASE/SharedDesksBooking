import React from "react";
import { User } from "../types";

interface IdentificationSectionProps {
    user: User;
    onUserChange: (user: User) => void;
}

const IdentificationSection: React.FC<IdentificationSectionProps> = ({
    user,
    onUserChange,
}) => {
    return (
        <div className="row justify-content-center mb-4">
            <div className="col-md-6 p-4 shadow-sm bg-light rounded border text-center">
                <h5 className="mb-3">Identify Yourself</h5>
                <div className="row g-2">
                    <div className="col">
                        <input
                            type="text"
                            className="form-control"
                            placeholder="First Name"
                            value={user.firstName}
                            onChange={(e) => onUserChange({ ...user, firstName: e.target.value })}
                        />
                    </div>
                    <div className="col">
                        <input
                            type="text"
                            className="form-control"
                            placeholder="Last Name"
                            value={user.lastName}
                            onChange={(e) => onUserChange({ ...user, lastName: e.target.value })}
                        />
                    </div>
                </div>
            </div>
        </div>
    );
};

export default IdentificationSection;
