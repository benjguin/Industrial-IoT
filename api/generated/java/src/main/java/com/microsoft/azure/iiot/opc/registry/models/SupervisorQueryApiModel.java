/**
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for
 * license information.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
 * Changes may cause incorrect behavior and will be lost if the code is
 * regenerated.
 */

package com.microsoft.azure.iiot.opc.registry.models;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Supervisor registration query.
 */
public class SupervisorQueryApiModel {
    /**
     * Site of the supervisor.
     */
    @JsonProperty(value = "siteId")
    private String siteId;

    /**
     * Discovery mode of supervisor. Possible values include: 'Off', 'Local',
     * 'Network', 'Fast', 'Scan'.
     */
    @JsonProperty(value = "discovery")
    private DiscoveryMode discovery;

    /**
     * Included connected or disconnected.
     */
    @JsonProperty(value = "connected")
    private Boolean connected;

    /**
     * Get site of the supervisor.
     *
     * @return the siteId value
     */
    public String siteId() {
        return this.siteId;
    }

    /**
     * Set site of the supervisor.
     *
     * @param siteId the siteId value to set
     * @return the SupervisorQueryApiModel object itself.
     */
    public SupervisorQueryApiModel withSiteId(String siteId) {
        this.siteId = siteId;
        return this;
    }

    /**
     * Get discovery mode of supervisor. Possible values include: 'Off', 'Local', 'Network', 'Fast', 'Scan'.
     *
     * @return the discovery value
     */
    public DiscoveryMode discovery() {
        return this.discovery;
    }

    /**
     * Set discovery mode of supervisor. Possible values include: 'Off', 'Local', 'Network', 'Fast', 'Scan'.
     *
     * @param discovery the discovery value to set
     * @return the SupervisorQueryApiModel object itself.
     */
    public SupervisorQueryApiModel withDiscovery(DiscoveryMode discovery) {
        this.discovery = discovery;
        return this;
    }

    /**
     * Get included connected or disconnected.
     *
     * @return the connected value
     */
    public Boolean connected() {
        return this.connected;
    }

    /**
     * Set included connected or disconnected.
     *
     * @param connected the connected value to set
     * @return the SupervisorQueryApiModel object itself.
     */
    public SupervisorQueryApiModel withConnected(Boolean connected) {
        this.connected = connected;
        return this;
    }

}
