import React from 'react';
import { Dialog, DialogContent, DialogTitle, DialogBody, DialogActions, Button } from "@fluentui/react-components";

interface ErrorDialogProps {
    errorMessage: string | null;
    isOpen: boolean;
    onClose: () => void;
}

const ErrorDialog: React.FC<ErrorDialogProps> = ({ errorMessage, isOpen, onClose }) => {
    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent>
                <DialogTitle>Error</DialogTitle>
                <DialogBody>{errorMessage}</DialogBody>
                <DialogActions>
                    <Button onClick={onClose}>Close</Button>
                </DialogActions>
            </DialogContent>
        </Dialog>
    );
};

export default ErrorDialog;