﻿using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;

namespace Stratis.Bitcoin.Networks.Deployments
{   
    /// <summary>
    /// BIP9 deployments for the Impleum network.
    /// </summary>
    public class ImpleumBIP9Deployments : BIP9DeploymentsArray
    {
        // The position of each deployment in the deployments array.
        public const int TestDummy = 0;

        // The number of deployments.
        public const int NumberOfDeployments = 1;

        /// <summary>
        /// Constructs the BIP9 deployments array.
        /// </summary>
        public ImpleumBIP9Deployments() : base(NumberOfDeployments)
        {
        }

        /// <summary>
        /// Gets the deployment flags to set when the deployment activates.
        /// </summary>
        /// <param name="deployment">The deployment number.</param>
        /// <returns>The deployment flags.</returns>
        public override BIP9DeploymentFlags GetFlags(int deployment)
        {
            return new BIP9DeploymentFlags();
        }
    }
}
