import React, { useState, useContext, ChangeEvent } from 'react';
import {
    Dialog,
    DialogTrigger,
    DialogSurface,
    DialogTitle,
    DialogBody,
    DialogActions,
    DialogContent,
    Button,
    Field,
    Input,
    makeStyles
} from "@fluentui/react-components";
import { TeamsFxContext } from '../Context';

const useStyles = makeStyles({
    formContent: {
        display: 'flex',
        flexWrap: 'wrap',
        justifyContent: 'space-around',
        alignItems: 'center',
    },
    formSurface: {
        width: '500px',
    },
    formButtons: {
        marginTop: '1rem',
    },
    field: {
        width: '200px',
    },
});

interface FormData {
    phoneNumber: string;
    restaurant: string;
    bankAccount: string;
    minimalPrice: string;
    deliveryFee: string;
    minimalPriceForFreeDelivery: string;
    closingTime: string;
}

interface FormErrors {
    phoneNumber?: string;
    restaurant?: string;
    bankAccount?: string;
    minimalPrice?: string;
    deliveryFee?: string;
    minimalPriceForFreeDelivery?: string;
    closingTime?: string;
}

const OrderForm: React.FC = () => {
    const { teamsUserCredential } = useContext(TeamsFxContext);
    const classes = useStyles();
    const [formData, setFormData] = useState<FormData>({
        phoneNumber: '',
        restaurant: '',
        bankAccount: '',
        minimalPrice: '',
        deliveryFee: '',
        minimalPriceForFreeDelivery: '',
        closingTime: ''
    });
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [errors, setErrors] = useState<FormErrors>({});

    const handleChange = (event: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = event.target;
        setFormData({ ...formData, [name]: value });
    };

    const validateForm = (): boolean => {
        const newErrors: FormErrors = {};

        if (!formData.phoneNumber) {
            newErrors.phoneNumber = 'Phone number is required.';
        } else if (formData.phoneNumber.length !== 9) {
            newErrors.phoneNumber = 'Phone number must be 9 characters.';
        }

        if (!formData.bankAccount) {
            newErrors.bankAccount = 'Bank account is required.';
        } else if (formData.bankAccount.length !== 26) {
            newErrors.bankAccount = 'Bank account must be 26 characters.';
        }

        if (!formData.restaurant) {
            newErrors.restaurant = 'Restaurant name is required.';
        }
        if (!formData.minimalPrice || parseFloat(formData.minimalPrice) < 0) {
            newErrors.minimalPrice = 'Minimal price cannot be negative.';
        }
        if (!formData.deliveryFee || parseFloat(formData.deliveryFee) < 0) {
            newErrors.deliveryFee = 'Delivery fee cannot be negative.';
        }
        if (!formData.minimalPriceForFreeDelivery || parseFloat(formData.minimalPriceForFreeDelivery) < 0) {
            newErrors.minimalPriceForFreeDelivery = 'Minimal price for free delivery cannot be negative.';
        }
        if (!formData.closingTime) {
            newErrors.closingTime = 'Set when you will close your order';
        }

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async () => {
        if (!validateForm()) return;

        try {
            if (!teamsUserCredential) return;
            const token = await teamsUserCredential.getToken("");

            const today = new Date();
            const [hours, minutes] = formData.closingTime.split(':');

            today.setHours(parseInt(hours), parseInt(minutes), 0, 0);

            const closingTimeISO = new Date(today.getTime() - today.getTimezoneOffset() * 60000).toISOString();

            const formattedData = {
                phoneNumber: formData.phoneNumber,
                bankAccount: formData.bankAccount,
                restaurant: formData.restaurant,
                minimalPrice: parseFloat(formData.minimalPrice),
                deliveryFee: parseFloat(formData.deliveryFee),
                minimalPriceForFreeDelivery: parseFloat(formData.minimalPriceForFreeDelivery),
                closingTime: closingTimeISO
            };

            const response = await fetch('https://localhost:7125/Order', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token!.token}`
                },
                body: JSON.stringify(formattedData)
            });

            if (response.ok) {
                setIsDialogOpen(false);
            } else {
                console.error(`Failed to submit form: ${response.statusText}`);
            }
        } catch (error) {
            console.error('Error during form submission:', error);
        }
    };

    return (
        <Dialog open={isDialogOpen} onOpenChange={(_, data) => setIsDialogOpen(data.open)}>
            <DialogTrigger disableButtonEnhancement>
                <Button onClick={() => setIsDialogOpen(true)}>Add Order</Button>
            </DialogTrigger>
            <DialogSurface className={classes.formSurface}>
                <DialogBody>
                    <DialogTitle>Order Form</DialogTitle>
                    <DialogContent className={classes.formContent}>
                        <Field className={classes.field}
                            label="Phone Number"
                            validationState={errors.phoneNumber ? 'error' : 'none'}
                            validationMessage={errors.phoneNumber}
                        >
                            <Input
                                id="phoneNumber"
                                name="phoneNumber"
                                value={formData.phoneNumber}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Restaurant Name"
                            validationState={errors.restaurant ? 'error' : 'none'}
                            validationMessage={errors.restaurant}
                        >
                            <Input
                                id="restaurant"
                                name="restaurant"
                                value={formData.restaurant}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Bank Account"
                            validationState={errors.bankAccount ? 'error' : 'none'}
                            validationMessage={errors.bankAccount}
                        >
                            <Input
                                id="bankAccount"
                                name="bankAccount"
                                value={formData.bankAccount}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Minimal Price"
                            validationState={errors.minimalPrice ? 'error' : 'none'}
                            validationMessage={errors.minimalPrice}
                        >
                            <Input
                                id="minimalPrice"
                                name="minimalPrice"
                                type="number"
                                value={formData.minimalPrice}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Delivery Fee"
                            validationState={errors.deliveryFee ? 'error' : 'none'}
                            validationMessage={errors.deliveryFee}
                        >
                            <Input
                                id="deliveryFee"
                                name="deliveryFee"
                                type="number"
                                value={formData.deliveryFee}
                                onChange={handleChange}
                                required
                            />
                        </Field>

                        <Field className={classes.field}
                            label="Minimal Price For Free Delivery"
                            validationState={errors.minimalPriceForFreeDelivery ? 'error' : 'none'}
                            validationMessage={errors.minimalPriceForFreeDelivery}
                        >
                            <Input
                                id="minimalPriceForFreeDelivery"
                                name="minimalPriceForFreeDelivery"
                                type="number"
                                value={formData.minimalPriceForFreeDelivery}
                                onChange={handleChange}
                                required
                            />
                        </Field>
                        <Field className={classes.field}
                            label="Closing Time"
                            validationState={errors.closingTime ? 'error' : 'none'}
                            validationMessage={errors.closingTime}
                        >
                            <Input
                                id="closingTime"
                                name="closingTime"
                                type="time"
                                value={formData.closingTime}
                                onChange={handleChange}
                                required
                            />
                        </Field>
                    </DialogContent>
                    <DialogActions className={classes.formButtons}>
                        <DialogTrigger disableButtonEnhancement>
                            <Button appearance="secondary">Close</Button>
                        </DialogTrigger>
                        <Button appearance="primary" onClick={handleSubmit}>Submit</Button>
                    </DialogActions>
                </DialogBody>
            </DialogSurface>
        </Dialog>
    );
}

export default OrderForm;
